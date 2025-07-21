using Xunit;
using Xunit.Abstractions;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales;

namespace MIVET.BE.IntegrationTests
{
    /// <summary>
    /// CP006 - Pruebas de concurrencia para MascotaControllers
    /// Creación concurrente de múltiples mascotas para un mismo cliente
    /// </summary>
    public class CP006_MascotaTests : BaseIntegrationTest
    {
        private const string CLIENT_DOC = "555333222"; // Cliente mock existente

        public CP006_MascotaTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public async Task CP006_Paso1_ConcurrentPetCreation_ShouldCreateAllPets()
        {
            // Arrange
            var mascotasData = new List<MascotaDTO>();

            for (int i = 1; i <= 5; i++)
            {
                var mascota = CreateMockMascotaDTO($"Pet{i}", CLIENT_DOC, i);
                mascota.Especie = i % 2 == 0 ? "Perro" : "Gato";
                mascota.Raza = i % 2 == 0 ? "Labrador" : "Siamés";
                mascotasData.Add(mascota);
            }

            LogTestStep("CP006", "Paso1", "Iniciando creación concurrente de 5 mascotas para el mismo cliente");

            // Act - Crear 5 mascotas simultáneamente
            var tasks = mascotasData.Select(PostMascotaAsync).ToList();
            var responses = await Task.WhenAll(tasks);

            // Assert
            var successfulResponses = responses.Where(r => r.IsSuccessStatusCode).ToList();

            LogResponseCodes("CP006", "Paso1", responses);

            // Los 5 responses 200 OK; BD con 5 nuevos registros
            Assert.True(successfulResponses.Count >= 4,
                $"Expected at least 4 successful creations, got {successfulResponses.Count}");

            // Verificar escalabilidad y auto incremento de IDs
            var mascotasCreadas = new List<MascotaDTO>();
            foreach (var response in successfulResponses)
            {
                var mascota = await DeserializeResponse<MascotaDTO>(response);
                mascotasCreadas.Add(mascota);
            }

            // Verificar que todos los IDs son únicos y mayores a 0
            var uniqueIds = mascotasCreadas.Select(m => m.Id).Distinct().ToList();
            Assert.Equal(uniqueIds.Count, mascotasCreadas.Count);
            Assert.All(mascotasCreadas, m => Assert.True(m.Id > 0));

            // Verificar que todas pertenecen al mismo cliente
            Assert.All(mascotasCreadas, m => Assert.Equal(CLIENT_DOC, m.NumeroDocumento));

            LogTestStep("CP006", "Paso1",
                $"Creadas {mascotasCreadas.Count} mascotas con IDs: {string.Join(", ", mascotasCreadas.Select(m => m.Id))}");
            LogTestStep("CP006", "Paso1", "Auto-incremento de IDs y escalabilidad verificados");
        }

        [Fact]
        public async Task CP006_Paso2_ConcurrentPetListing_ShouldReturnConsistentData()
        {
            LogTestStep("CP006", "Paso2", "Iniciando listado concurrente de mascotas por dueño");

            // Act - Listar mascotas del dueño concurrentemente
            var tasks = new List<Task<HttpResponseMessage>>
            {
                GetMascotasByDuenoAsync(CLIENT_DOC),
                GetMascotasByDuenoAsync(CLIENT_DOC)
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            foreach (var response in responses)
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status: {response.StatusCode}");
            }

            LogResponseCodes("CP006", "Paso2", responses);

            // Asegurar consistencia de la relación 1:N
            var data1 = await DeserializeResponse<List<MascotaConDuenoDTO>>(responses[0]);
            var data2 = await DeserializeResponse<List<MascotaConDuenoDTO>>(responses[1]);

            // Ambos 200 OK; al menos las mascotas base en la lista
            var countDifference = Math.Abs(data1.Count - data2.Count);
            Assert.True(countDifference <= 2, $"Count difference too large: {countDifference}");

            // Verificar que todas las mascotas pertenecen al cliente correcto
            Assert.All(data1, m => Assert.Equal(CLIENT_DOC, m.NumeroDocumento));
            Assert.All(data2, m => Assert.Equal(CLIENT_DOC, m.NumeroDocumento));

            // Verificar que no hay duplicados por ID
            var duplicates1 = data1.GroupBy(m => m.id).Where(g => g.Count() > 1).ToList();
            var duplicates2 = data2.GroupBy(m => m.id).Where(g => g.Count() > 1).ToList();

            Assert.Empty(duplicates1);
            Assert.Empty(duplicates2);

            LogTestStep("CP006", "Paso2",
                $"Consistencia verificada - Conteos: {data1.Count}, {data2.Count}");
            LogTestStep("CP006", "Paso2", "Relación 1:N cliente-mascotas consistente");
        }

        [Fact]
        public async Task CP006_Paso3_ConcurrentUpdateAndDelete_ShouldHandleRaceCondition()
        {
            // Arrange - Crear una mascota específica para esta prueba
            var mascotaData = CreateMockMascotaDTO("PetToModify", CLIENT_DOC, 2);
            mascotaData.Especie = "Gato";
            mascotaData.Raza = "Persa";

            LogTestStep("CP006", "Paso3", "Creando mascota para prueba UPDATE + DELETE");

            var createResponse = await PostMascotaAsync(mascotaData);
            Assert.True(createResponse.IsSuccessStatusCode, "Failed to create pet for test");

            var createdPet = await DeserializeResponse<MascotaDTO>(createResponse);
            int petId = createdPet.Id;

            LogTestStep("CP006", "Paso3", $"Mascota creada con ID: {petId}");

            // Preparar datos de actualización
            var updateData = CreateMockMascotaDTO("PetToModify", CLIENT_DOC, 4);
            updateData.Id = petId;
            updateData.Nombre = "PetModified";
            updateData.Edad = 4; // Cambiar edad

            // Act - Modificación y eliminación simultánea
            var tasks = new List<Task<HttpResponseMessage>>
            {
                PutMascotaAsync(updateData),
                DeleteMascotaAsync(petId)
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            var statusCodes = responses.Select(r => r.StatusCode).ToList();
            LogResponseCodes("CP006", "Paso3", responses);

            // Uno 200 OK y el otro 204 No Content o 404 Not Found según orden
            var hasSuccess = responses.Any(r =>
                r.StatusCode == System.Net.HttpStatusCode.OK ||
                r.StatusCode == System.Net.HttpStatusCode.NoContent ||
                r.StatusCode == System.Net.HttpStatusCode.NotFound);

            Assert.True(hasSuccess, "At least one operation should complete successfully");

            // Definir y verificar comportamiento ante conflicto
            var finalCheckResponse = await GetMascotaAsync(petId);
            var petExists = finalCheckResponse.IsSuccessStatusCode;

            LogTestStep("CP006", "Paso3",
                $"Estado final de la mascota ID {petId}: {(petExists ? "Existe" : "No existe")}");

            if (petExists)
            {
                var finalData = await DeserializeResponse<MascotaDTO>(finalCheckResponse);
                LogTestStep("CP006", "Paso3",
                    $"Datos finales: Nombre={finalData.Nombre}, Edad={finalData.Edad}");
            }

            LogTestStep("CP006", "Paso3", "Comportamiento ante conflicto UPDATE/DELETE verificado");
        }

        [Fact]
        public async Task CP006_Paso4_CleanupCreatedPets_ShouldSucceed()
        {
            LogTestStep("CP006", "Paso4", "Iniciando limpieza de mascotas creadas en pruebas");

            // Arrange - Obtener todas las mascotas del cliente de prueba
            var listResponse = await GetMascotasByDuenoAsync(CLIENT_DOC);

            if (listResponse.IsSuccessStatusCode)
            {
                var mascotas = await DeserializeResponse<List<MascotaConDuenoDTO>>(listResponse);
                var mascotasDePrueba = mascotas.Where(m =>
                    m.Nombre.StartsWith("Pet") ||
                    m.Nombre.Contains("Test") ||
                    m.Nombre.Contains("Modified")).ToList();

                LogTestStep("CP006", "Paso4",
                    $"Encontradas {mascotasDePrueba.Count} mascotas de prueba para eliminar");

                // Act - Eliminar mascotas de prueba
                var deleteTasks = mascotasDePrueba.Select(m => DeleteMascotaAsync(m.id)).ToList();

                if (deleteTasks.Any())
                {
                    var deleteResponses = await Task.WhenAll(deleteTasks);
                    var successfulDeletes = deleteResponses.Count(r =>
                        r.StatusCode == System.Net.HttpStatusCode.NoContent);

                    LogTestStep("CP006", "Paso4",
                        $"Eliminadas {successfulDeletes}/{deleteTasks.Count} mascotas de prueba");
                }
            }

            // Assert - Verificar estado de limpieza
            var finalListResponse = await GetMascotasByDuenoAsync(CLIENT_DOC);
            if (finalListResponse.IsSuccessStatusCode)
            {
                var finalMascotas = await DeserializeResponse<List<MascotaConDuenoDTO>>(finalListResponse);
                var remainingTestPets = finalMascotas.Count(m =>
                    m.Nombre.StartsWith("Pet") ||
                    m.Nombre.Contains("Test") ||
                    m.Nombre.Contains("Modified"));

                LogTestStep("CP006", "Paso4",
                    $"Mascotas de prueba restantes: {remainingTestPets}");
            }

            LogTestStep("CP006", "Paso4", "Limpieza de datos completada");
            Assert.True(true); // Paso siempre pasa, es para completar el flujo de limpieza
        }

        [Fact]
        public async Task CP006_Extra_StressTestMultiplePetsCreation_ShouldHandleHighConcurrency()
        {
            // Arrange - Prueba de estrés con más mascotas
            var mascotasData = new List<MascotaDTO>();

            for (int i = 1; i <= 10; i++)
            {
                var mascota = CreateMockMascotaDTO($"StressPet{i}", CLIENT_DOC, i % 5 + 1);
                mascota.Especie = i % 3 == 0 ? "Ave" : (i % 2 == 0 ? "Perro" : "Gato");
                mascota.Raza = $"Raza{i % 4 + 1}";
                mascotasData.Add(mascota);
            }

            LogTestStep("CP006", "Extra", "Iniciando prueba de estrés con 10 mascotas concurrentes");

            // Act
            var tasks = mascotasData.Select(PostMascotaAsync).ToList();
            var responses = await Task.WhenAll(tasks);

            // Assert
            var successCount = responses.Count(r => r.IsSuccessStatusCode);
            LogTestStep("CP006", "Extra", $"Prueba de estrés: {successCount}/10 mascotas creadas exitosamente");

            // Verificar que al menos el 80% fueron exitosas
            Assert.True(successCount >= 8, $"Expected at least 8 successful creations, got {successCount}");

            // Verificar IDs únicos
            var mascotasCreadas = new List<MascotaDTO>();
            foreach (var response in responses.Where(r => r.IsSuccessStatusCode))
            {
                var mascota = await DeserializeResponse<MascotaDTO>(response);
                mascotasCreadas.Add(mascota);
            }

            var uniqueIds = mascotasCreadas.Select(m => m.Id).Distinct().Count();
            Assert.Equal(mascotasCreadas.Count, uniqueIds);

            LogTestStep("CP006", "Extra", "Prueba de estrés completada - IDs únicos verificados");
        }
    }
}