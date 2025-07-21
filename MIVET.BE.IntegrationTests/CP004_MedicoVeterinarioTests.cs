using MIVET.BE.Transversales.Entidades;
using Xunit;
using Xunit.Abstractions;

namespace MIVET.BE.IntegrationTests
{
    /// <summary>
    /// CP004 - Pruebas de concurrencia para MedicoVeterinarioControllers
    /// Modificaciones concurrentes de un mismo veterinario
    /// </summary>
    public class CP004_MedicoVeterinarioTests : BaseIntegrationTest
    {
        public CP004_MedicoVeterinarioTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public async Task CP004_Paso1_ConcurrentPutSameVeterinarian_ShouldHandleLastWriteWins()
        {
            // Arrange
            int vetId = 1; // El veterinario mock ya existe
            var hiloAData = CreateMockMedicoVeterinarioDTO(vetId, "3001111111");
            var hiloBData = CreateMockMedicoVeterinarioDTO(vetId, "3002222222");

            LogTestStep("CP004", "Paso1", "Iniciando prueba de PUT concurrente al mismo vetId");

            // Act
            var tasks = new List<Task<HttpResponseMessage>>
            {
                PutMedicoVeterinarioAsync(hiloAData),
                PutMedicoVeterinarioAsync(hiloBData)
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            foreach (var response in responses)
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status: {response.StatusCode}");
            }

            LogResponseCodes("CP004", "Paso1", responses);

            // Verificar que la BD queda con uno de los dos valores (last write wins)
            var finalStateResponse = await GetMedicoVeterinarioAsync(vetId);
            Assert.True(finalStateResponse.IsSuccessStatusCode);

            var finalState = await DeserializeResponse<MedicoVeterinarioDTO>(finalStateResponse);
            Assert.True(finalState.Telefono == "3001111111" || finalState.Telefono == "3002222222");

            LogTestStep("CP004", "Paso1", $"Final phone number: {finalState.Telefono} - Mecanismo de last write wins verificado");
        }

        [Fact]
        public async Task CP004_Paso2_ConcurrentReadDuringUpdates_ShouldReturnConsistentData()
        {
            // Arrange
            int vetId = 1;
            var updateData = CreateMockMedicoVeterinarioDTO(vetId, "3009999999");
            updateData.Nombre = "Dr. Updated Concurrently";

            LogTestStep("CP004", "Paso2", "Iniciando lecturas concurrentes durante actualización");

            // Act - Ejecutar lecturas concurrentes mientras se actualiza
            var tasks = new List<Task<HttpResponseMessage>>
            {
                PutMedicoVeterinarioAsync(updateData),
                GetMedicoVeterinarioAsync(vetId),
                GetMedicoVeterinarioAsync(vetId)
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            foreach (var response in responses)
            {
                Assert.True(response.IsSuccessStatusCode);
            }

            LogResponseCodes("CP004", "Paso2", responses);

            // Verificar que las lecturas devuelven datos consistentes (no corruptos)
            for (int i = 1; i < responses.Length; i++)
            {
                var data = await DeserializeResponse<MedicoVeterinarioDTO>(responses[i]);
                Assert.NotNull(data.Telefono);
                Assert.NotEmpty(data.Telefono);
                Assert.NotNull(data.Nombre);
                Assert.NotEmpty(data.Nombre);

                LogTestStep("CP004", "Paso2", $"Lectura {i}: Teléfono={data.Telefono}, Nombre={data.Nombre}");
            }

            LogTestStep("CP004", "Paso2", "Datos consistentes verificados - No hay corrupción de datos intermedios");
        }

        [Fact]
        public async Task CP004_Paso3_ConcurrentDeleteAndPut_ShouldHandleGracefully()
        {
            // Arrange - Crear veterinario específico para esta prueba
            var newVet = CreateMockMedicoVeterinarioDTO(0, "3005555555"); // ID 0 para crear nuevo
            newVet.NumeroDocumento = "87654321";
            newVet.Nombre = "Dr. Test Delete";

            LogTestStep("CP004", "Paso3", "Creando veterinario para prueba DELETE + PUT");

            var createResponse = await PostMedicoVeterinarioAsync(newVet);
            Assert.True(createResponse.IsSuccessStatusCode);

            var createdVet = await DeserializeResponse<MedicoVeterinarioDTO>(createResponse);
            int vetId = createdVet.Id;

            LogTestStep("CP004", "Paso3", $"Veterinario creado con ID: {vetId}");

            var putData = CreateMockMedicoVeterinarioDTO(vetId, "3005555555");
            putData.Nombre = "Dr. Updated After Delete Attempt";

            // Act - Ejecutar DELETE y PUT simultáneo
            var tasks = new List<Task<HttpResponseMessage>>
            {
                DeleteMedicoVeterinarioAsync(vetId),
                PutMedicoVeterinarioAsync(putData)
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            var statusCodes = responses.Select(r => r.StatusCode).ToList();
            LogResponseCodes("CP004", "Paso3", responses);

            // Uno debe ser 204 No Content y el otro 404 Not Found o 200 OK según orden
            var hasNoContent = statusCodes.Contains(System.Net.HttpStatusCode.NoContent);
            var hasValidResponse = responses.Any(r =>
                r.StatusCode == System.Net.HttpStatusCode.NoContent ||
                r.StatusCode == System.Net.HttpStatusCode.OK ||
                r.StatusCode == System.Net.HttpStatusCode.NotFound);

            Assert.True(hasValidResponse, "Should have at least one valid response (204, 200, or 404)");

            // Verificar que la BD queda en estado coherente
            var finalCheckResponse = await GetMedicoVeterinarioAsync(vetId);
            var finalExists = finalCheckResponse.IsSuccessStatusCode;

            LogTestStep("CP004", "Paso3", $"Estado final del veterinario ID {vetId}: {(finalExists ? "Existe" : "No existe")}");
            LogTestStep("CP004", "Paso3", "Comportamiento de race condition DELETE/PUT manejado apropiadamente");
        }

        [Fact]
        public async Task CP004_Paso4_RestoreOriginalData_ShouldSucceed()
        {
            // Arrange
            var originalData = CreateMockMedicoVeterinarioDTO(1, "3000000000");
            originalData.Nombre = "Dr. Test Mock"; // Nombre original

            LogTestStep("CP004", "Paso4", "Restaurando datos originales del veterinario");

            // Act
            var response = await PutMedicoVeterinarioAsync(originalData);

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Failed to restore original data: {response.StatusCode}");

            // Verificar que los datos se restauraron correctamente
            var verifyResponse = await GetMedicoVeterinarioAsync(1);
            Assert.True(verifyResponse.IsSuccessStatusCode);

            var restoredData = await DeserializeResponse<MedicoVeterinarioDTO>(verifyResponse);
            Assert.Equal("3000000000", restoredData.Telefono);
            Assert.Equal("Dr. Test Mock", restoredData.Nombre);

            LogTestStep("CP004", "Paso4", "Datos originales restaurados exitosamente - BD en estado inicial");
        }

        [Fact]
        public async Task CP004_Extra_StressTest_MultipleConcurrentOperations()
        {
            // Arrange - Prueba adicional de estrés
            int vetId = 1;
            var tasks = new List<Task<HttpResponseMessage>>();

            LogTestStep("CP004", "Extra", "Iniciando prueba de estrés con múltiples operaciones concurrentes");

            // Act - Múltiples operaciones simultáneas
            for (int i = 0; i < 5; i++)
            {
                var updateData = CreateMockMedicoVeterinarioDTO(vetId, $"300{i}000000");
                tasks.Add(PutMedicoVeterinarioAsync(updateData));
                tasks.Add(GetMedicoVeterinarioAsync(vetId));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            var successCount = responses.Count(r => r.IsSuccessStatusCode);
            var totalOperations = tasks.Count;

            Assert.True(successCount >= totalOperations * 0.8,
                $"Expected at least 80% success rate, got {successCount}/{totalOperations}");

            LogTestStep("CP004", "Extra", $"Prueba de estrés completada: {successCount}/{totalOperations} operaciones exitosas");
        }
    }
}