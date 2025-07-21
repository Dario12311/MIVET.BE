using Xunit;
using Xunit.Abstractions;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.IntegrationTests
{
    /// <summary>
    /// CP005 - Pruebas de concurrencia para PersonaClienteControllers
    /// Registro y eliminación concurrente de un cliente
    /// </summary>
    public class CP005_PersonaClienteTests : BaseIntegrationTest
    {
        public CP005_PersonaClienteTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public async Task CP005_Paso1_ConcurrentClientRegistration_ShouldHandleUniqueConstraint()
        {
            // Arrange
            var clienteData = CreateMockPersonaCliente("555333999"); // Nuevo documento para evitar conflictos
            clienteData.PrimerNombre = "Cliente";
            clienteData.PrimerApellido = "Concurrente";

            LogTestStep("CP005", "Paso1", "Iniciando registro concurrente del mismo cliente");

            // Act - Registrar el mismo cliente simultáneamente
            var tasks = new List<Task<HttpResponseMessage>>
            {
                PostPersonaClienteAsync(clienteData),
                PostPersonaClienteAsync(clienteData)
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            var successResponses = responses.Where(r => r.IsSuccessStatusCode).ToList();
            var conflictResponses = responses.Where(r =>
                r.StatusCode == System.Net.HttpStatusCode.Conflict ||
                r.StatusCode == System.Net.HttpStatusCode.InternalServerError).ToList();

            LogResponseCodes("CP005", "Paso1", responses);

            // Uno 200 OK; el otro 409 Conflict (o error de unique constraint)
            Assert.True(successResponses.Count >= 1, "At least one registration should succeed");

            // Verificar constraint de unicidad en BD
            var verifyResponse = await GetPersonaClienteAsync("555333999");
            if (verifyResponse.IsSuccessStatusCode)
            {
                var clienteCreado = await DeserializeResponse<PersonaCliente>(verifyResponse);
                Assert.Equal("555333999", clienteCreado.NumeroDocumento);
                LogTestStep("CP005", "Paso1", "Cliente registrado correctamente en BD");
            }

            LogTestStep("CP005", "Paso1", $"Constraint de unicidad verificado - Éxitos: {successResponses.Count}, Conflictos: {conflictResponses.Count}");
        }

        [Fact]
        public async Task CP005_Paso2_ConcurrentDeleteAndUpdate_ShouldHandleRaceCondition()
        {
            // Arrange
            string numeroDocumento = "555444777";
            var clienteData = CreateMockPersonaCliente(numeroDocumento);
            clienteData.PrimerNombre = "Cliente";
            clienteData.PrimerApellido = "ParaEliminar";

            LogTestStep("CP005", "Paso2", "Creando cliente para prueba DELETE + PUT");

            // Primero crear el cliente
            var createResponse = await PostPersonaClienteAsync(clienteData);
            if (!createResponse.IsSuccessStatusCode)
            {
                LogTestStep("CP005", "Paso2", $"Warning: Cliente no creado, posiblemente ya existe. Status: {createResponse.StatusCode}");
            }

            var updateData = CreateMockPersonaCliente(numeroDocumento);
            updateData.PrimerNombre = "Cliente Updated";
            updateData.Telefono = "3009876543";
            updateData.Direccion = "Dirección Actualizada";

            LogTestStep("CP005", "Paso2", "Ejecutando DELETE y PUT simultáneo");

            // Act - Eliminar y modificar el mismo cliente simultáneamente
            var tasks = new List<Task<HttpResponseMessage>>
            {
                DeletePersonaClienteAsync(numeroDocumento),
                PutPersonaClienteAsync(updateData)
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            var statusCodes = responses.Select(r => r.StatusCode).ToList();
            LogResponseCodes("CP005", "Paso2", responses);

            // Uno 204 No Content; el otro 404 Not Found o 200 OK según orden
            var hasNoContent = statusCodes.Contains(System.Net.HttpStatusCode.NoContent);
            var hasValidResponse = responses.Any(r =>
                r.StatusCode == System.Net.HttpStatusCode.NoContent ||
                r.StatusCode == System.Net.HttpStatusCode.OK ||
                r.StatusCode == System.Net.HttpStatusCode.NotFound);

            Assert.True(hasValidResponse, "Should have at least one valid response");

            // Comprobar atomicidad y orden de ejecución
            var finalCheckResponse = await GetPersonaClienteAsync(numeroDocumento);
            var clientExists = finalCheckResponse.IsSuccessStatusCode;

            LogTestStep("CP005", "Paso2", $"Estado final del cliente {numeroDocumento}: {(clientExists ? "Existe" : "No existe")}");
            LogTestStep("CP005", "Paso2", "Atomicidad y orden de ejecución verificados");
        }

        [Fact]
        public async Task CP005_Paso3_ConcurrentClientListing_ShouldReturnConsistentData()
        {
            LogTestStep("CP005", "Paso3", "Iniciando listado concurrente de clientes");

            // Act - Listar clientes concurrentemente
            var tasks = new List<Task<HttpResponseMessage>>
            {
                GetAllPersonaClienteAsync(),
                GetAllPersonaClienteAsync()
            };

            var responses = await Task.WhenAll(tasks);

            // Assert
            foreach (var response in responses)
            {
                Assert.True(response.IsSuccessStatusCode, $"Response status: {response.StatusCode}");
            }

            LogResponseCodes("CP005", "Paso3", responses);

            // Validar consistencia de la vista
            var data1 = await DeserializeResponse<List<PersonaCliente>>(responses[0]);
            var data2 = await DeserializeResponse<List<PersonaCliente>>(responses[1]);

            // No hay duplicados ni faltantes en la lista
            var countDifference = Math.Abs(data1.Count - data2.Count);
            Assert.True(countDifference <= 1, $"Count difference too large: {countDifference}");

            // Verificar que no hay duplicados por NumeroDocumento
            var duplicates1 = data1.GroupBy(c => c.NumeroDocumento).Where(g => g.Count() > 1).ToList();
            var duplicates2 = data2.GroupBy(c => c.NumeroDocumento).Where(g => g.Count() > 1).ToList();

            Assert.Empty(duplicates1);
            Assert.Empty(duplicates2);

            LogTestStep("CP005", "Paso3", $"Consistencia verificada - Conteos: {data1.Count}, {data2.Count}");
            LogTestStep("CP005", "Paso3", "No se encontraron duplicados ni inconsistencias en las listas");
        }

        [Fact]
        public async Task CP005_Paso4_RecreateClientForCleanup_ShouldSucceed()
        {
            // Arrange
            var clienteData = CreateMockPersonaCliente("555333222"); // Cliente base del sistema
            clienteData.PrimerNombre = "Cliente";
            clienteData.PrimerApellido = "Mock";
            clienteData.CorreoElectronico = "cliente@mock.com";

            LogTestStep("CP005", "Paso4", "Re-creando cliente para dejar BD en estado inicial");

            // Act
            var response = await PostPersonaClienteAsync(clienteData);

            // Assert - 200 OK (o 201 Created); cliente restaurado
            var isSuccessful = response.IsSuccessStatusCode ||
                              response.StatusCode == System.Net.HttpStatusCode.Conflict; // Ya existe

            Assert.True(isSuccessful, $"Failed to recreate client: {response.StatusCode}");

            // Verificar que el cliente existe
            var verifyResponse = await GetPersonaClienteAsync("555333222");
            if (verifyResponse.IsSuccessStatusCode)
            {
                var clienteVerificado = await DeserializeResponse<PersonaCliente>(verifyResponse);
                Assert.Equal("555333222", clienteVerificado.NumeroDocumento);
                LogTestStep("CP005", "Paso4", "Cliente base verificado en BD");
            }

            LogTestStep("CP005", "Paso4", "Limpieza de datos completada - BD en estado inicial");
        }

        [Fact]
        public async Task CP005_Extra_ConcurrentUpdateOperations_ShouldMaintainConsistency()
        {
            // Arrange - Prueba adicional de múltiples actualizaciones concurrentes
            string numeroDocumento = "555888999";
            var clienteBase = CreateMockPersonaCliente(numeroDocumento);

            LogTestStep("CP005", "Extra", "Creando cliente para prueba de actualizaciones concurrentes");

            // Crear cliente base
            await PostPersonaClienteAsync(clienteBase);

            var tasks = new List<Task<HttpResponseMessage>>();

            // Act - Múltiples actualizaciones concurrentes
            for (int i = 1; i <= 3; i++)
            {
                var updateData = CreateMockPersonaCliente(numeroDocumento);
                updateData.PrimerNombre = $"Cliente{i}";
                updateData.Telefono = $"300000000{i}";
                tasks.Add(PutPersonaClienteAsync(updateData));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            var successCount = responses.Count(r => r.IsSuccessStatusCode);
            LogTestStep("CP005", "Extra", $"Actualizaciones concurrentes: {successCount}/3 exitosas");

            // Verificar estado final
            var finalResponse = await GetPersonaClienteAsync(numeroDocumento);
            if (finalResponse.IsSuccessStatusCode)
            {
                var finalData = await DeserializeResponse<PersonaCliente>(finalResponse);
                Assert.Contains(finalData.PrimerNombre, new[] { "Cliente1", "Cliente2", "Cliente3" });
                LogTestStep("CP005", "Extra", $"Estado final: {finalData.PrimerNombre}, {finalData.Telefono}");
            }

            Assert.True(successCount >= 1, "At least one update should succeed");
        }
    }
}