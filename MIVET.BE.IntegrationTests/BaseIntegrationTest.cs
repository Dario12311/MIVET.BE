using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales;
using System.Collections.Concurrent;

namespace MIVET.BE.IntegrationTests
{
    /// <summary>
    /// Clase base para todas las pruebas de integración con funcionalidad mock completa
    /// </summary>
    public abstract class BaseIntegrationTest : IDisposable
    {
        protected readonly MockHttpService _mockService;
        protected readonly ITestOutputHelper _output;

        protected BaseIntegrationTest(ITestOutputHelper output)
        {
            _mockService = new MockHttpService();
            _output = output;
        }

        #region Mock Data Factory Methods

        protected MedicoVeterinarioDTO CreateMockMedicoVeterinarioDTO(int id, string telefono)
        {
            return new MedicoVeterinarioDTO
            {
                Id = id,
                Telefono = telefono,
                Nombre = "Dr. Test Mock",
                NumeroDocumento = "12345678",
                EstadoCivil = 1,
                TipoDocumentoId = 1,
                Especialidad = "Veterinaria General",
                CorreoElectronico = "test@mock.com",
                Direccion = "Calle Mock 123",
                UniversidadGraduacion = "Universidad Mock",
                nacionalidad = "Colombiana",
                genero = "M",
                ciudad = "Bogotá",
                Estado = "A"
            };
        }

        protected PersonaCliente CreateMockPersonaCliente(string numeroDocumento)
        {
            return new PersonaCliente
            {
                NumeroDocumento = numeroDocumento,
                IdTipoDocumento = 1,
                PrimerNombre = "Juan",
                SegundoNombre = "Carlos",
                PrimerApellido = "Pérez",
                SegundoApellido = "García",
                CorreoElectronico = "juan.perez@mock.com",
                Telefono = "3001234567",
                Celular = "3001234567",
                Direccion = "Calle Mock 789",
                Ciudad = "Bogotá",
                Departamento = "Cundinamarca",
                Pais = "Colombia",
                CodigoPostal = "110111",
                Genero = "M",
                EstadoCivil = "Soltero",
                FechaNacimiento = "1990-01-01",
                LugarNacimiento = "Bogotá",
                Nacionalidad = "Colombiana",
                FechaRegistro = DateTime.Now,
                Estado = "A"
            };
        }

        protected MascotaDTO CreateMockMascotaDTO(string nombre, string numeroDocumento, int edad)
        {
            return new MascotaDTO
            {
                Nombre = nombre,
                Especie = "Perro",
                Raza = "Labrador",
                Edad = edad,
                Genero = edad % 2 == 0 ? "M" : "F",
                NumeroDocumento = numeroDocumento,
                Estado = 'A'
            };
        }

        #endregion

        #region MedicoVeterinario HTTP Methods

        protected async Task<HttpResponseMessage> PostMedicoVeterinarioAsync(MedicoVeterinarioDTO data)
        {
            return await _mockService.PostMedicoVeterinarioAsync(data);
        }

        protected async Task<HttpResponseMessage> PutMedicoVeterinarioAsync(MedicoVeterinarioDTO data)
        {
            return await _mockService.PutMedicoVeterinarioAsync(data);
        }

        protected async Task<HttpResponseMessage> GetMedicoVeterinarioAsync(int id)
        {
            return await _mockService.GetMedicoVeterinarioAsync(id);
        }

        protected async Task<HttpResponseMessage> DeleteMedicoVeterinarioAsync(int id)
        {
            return await _mockService.DeleteMedicoVeterinarioAsync(id);
        }

        #endregion

        #region PersonaCliente HTTP Methods

        protected async Task<HttpResponseMessage> PostPersonaClienteAsync(PersonaCliente data)
        {
            return await _mockService.PostPersonaClienteAsync(data);
        }

        protected async Task<HttpResponseMessage> PutPersonaClienteAsync(PersonaCliente data)
        {
            return await _mockService.PutPersonaClienteAsync(data);
        }

        protected async Task<HttpResponseMessage> DeletePersonaClienteAsync(string numeroDocumento)
        {
            return await _mockService.DeletePersonaClienteAsync(numeroDocumento);
        }

        protected async Task<HttpResponseMessage> GetAllPersonaClienteAsync()
        {
            return await _mockService.GetAllPersonaClienteAsync();
        }

        protected async Task<HttpResponseMessage> GetPersonaClienteAsync(string numeroDocumento)
        {
            return await _mockService.GetPersonaClienteAsync(numeroDocumento);
        }

        #endregion

        #region Mascota HTTP Methods

        protected async Task<HttpResponseMessage> PostMascotaAsync(MascotaDTO data)
        {
            return await _mockService.PostMascotaAsync(data);
        }

        protected async Task<HttpResponseMessage> PutMascotaAsync(MascotaDTO data)
        {
            return await _mockService.PutMascotaAsync(data);
        }

        protected async Task<HttpResponseMessage> DeleteMascotaAsync(int id)
        {
            return await _mockService.DeleteMascotaAsync(id);
        }

        protected async Task<HttpResponseMessage> GetMascotasByDuenoAsync(string numeroDocumento)
        {
            return await _mockService.GetMascotasByDuenoAsync(numeroDocumento);
        }

        protected async Task<HttpResponseMessage> GetMascotaAsync(int id)
        {
            return await _mockService.GetMascotaAsync(id);
        }

        #endregion

        #region Utility Methods

        protected async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        protected void LogTestStep(string testCase, string step, string message)
        {
            _output.WriteLine($"{testCase}_{step} - {message}");
        }

        protected void LogResponseCodes(string testCase, string step, IEnumerable<HttpResponseMessage> responses)
        {
            var statusCodes = responses.Select(r => r.StatusCode).ToList();
            _output.WriteLine($"{testCase}_{step} - Response codes: {string.Join(", ", statusCodes)}");
        }

        #endregion

        public virtual void Dispose()
        {
            _mockService?.Dispose();
        }
    }

    #region Mock HTTP Service - Simula comportamiento de API sin servidor real

    public class MockHttpService : IDisposable
    {
        private readonly ConcurrentDictionary<int, MedicoVeterinarioDTO> _veterinarios;
        private readonly ConcurrentDictionary<string, PersonaCliente> _clientes;
        private readonly ConcurrentDictionary<int, MascotaDTO> _mascotas;
        private int _nextVetId = 2;
        private int _nextMascotaId = 1;
        private readonly Random _random = new Random();

        public MockHttpService()
        {
            _veterinarios = new ConcurrentDictionary<int, MedicoVeterinarioDTO>();
            _clientes = new ConcurrentDictionary<string, PersonaCliente>();
            _mascotas = new ConcurrentDictionary<int, MascotaDTO>();

            InitializeData();
        }

        private void InitializeData()
        {
            // Veterinario inicial
            _veterinarios.TryAdd(1, new MedicoVeterinarioDTO
            {
                Id = 1,
                Nombre = "Dr. Test Mock",
                NumeroDocumento = "12345678",
                Telefono = "3000000000",
                EstadoCivil = 1,
                TipoDocumentoId = 1,
                Especialidad = "Veterinaria General",
                CorreoElectronico = "test@mock.com",
                Direccion = "Calle Mock 123",
                UniversidadGraduacion = "Universidad Mock",
                nacionalidad = "Colombiana",
                genero = "M",
                ciudad = "Bogotá",
                Estado = "A"
            });

            // Cliente inicial
            _clientes.TryAdd("555333222", new PersonaCliente
            {
                NumeroDocumento = "555333222",
                IdTipoDocumento = 1,
                PrimerNombre = "Cliente",
                PrimerApellido = "Mock",
                CorreoElectronico = "cliente@mock.com",
                Telefono = "3001111111",
                Celular = "3001111111",
                Direccion = "Calle Cliente 456",
                Ciudad = "Bogotá",
                Departamento = "Cundinamarca",
                Pais = "Colombia",
                Genero = "M",
                EstadoCivil = "Soltero",
                FechaNacimiento = "1985-05-15",
                LugarNacimiento = "Bogotá",
                Nacionalidad = "Colombiana",
                FechaRegistro = DateTime.Now,
                Estado = "A"
            });
        }

        #region MedicoVeterinario Mock Methods

        public async Task<HttpResponseMessage> GetMedicoVeterinarioAsync(int id)
        {
            await SimulateDelay();

            if (_veterinarios.TryGetValue(id, out var veterinario))
            {
                var json = JsonSerializer.Serialize(veterinario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<HttpResponseMessage> PostMedicoVeterinarioAsync(MedicoVeterinarioDTO data)
        {
            await SimulateDelay();

            var newId = Interlocked.Increment(ref _nextVetId);
            data.Id = newId;

            _veterinarios.TryAdd(newId, data);

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
        }

        public async Task<HttpResponseMessage> PutMedicoVeterinarioAsync(MedicoVeterinarioDTO data)
        {
            await SimulateDelay();

            if (_veterinarios.ContainsKey(data.Id))
            {
                _veterinarios.AddOrUpdate(data.Id, data, (key, oldValue) => data);

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<HttpResponseMessage> DeleteMedicoVeterinarioAsync(int id)
        {
            await SimulateDelay();

            if (_veterinarios.TryRemove(id, out _))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        #endregion

        #region PersonaCliente Mock Methods

        public async Task<HttpResponseMessage> GetPersonaClienteAsync(string numeroDocumento)
        {
            await SimulateDelay();

            if (_clientes.TryGetValue(numeroDocumento, out var cliente))
            {
                var json = JsonSerializer.Serialize(cliente);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<HttpResponseMessage> GetAllPersonaClienteAsync()
        {
            await SimulateDelay();

            var clientes = _clientes.Values.ToList();
            var json = JsonSerializer.Serialize(clientes);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
        }

        public async Task<HttpResponseMessage> PostPersonaClienteAsync(PersonaCliente data)
        {
            await SimulateDelay();

            // Simular constraint de unicidad
            if (_clientes.ContainsKey(data.NumeroDocumento))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.Conflict);
            }

            if (_clientes.TryAdd(data.NumeroDocumento, data))
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Conflict);
        }

        public async Task<HttpResponseMessage> PutPersonaClienteAsync(PersonaCliente data)
        {
            await SimulateDelay();

            if (_clientes.ContainsKey(data.NumeroDocumento))
            {
                _clientes.AddOrUpdate(data.NumeroDocumento, data, (key, oldValue) => data);

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<HttpResponseMessage> DeletePersonaClienteAsync(string numeroDocumento)
        {
            await SimulateDelay();

            if (_clientes.TryRemove(numeroDocumento, out _))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        #endregion

        #region Mascota Mock Methods

        public async Task<HttpResponseMessage> GetMascotaAsync(int id)
        {
            await SimulateDelay();

            if (_mascotas.TryGetValue(id, out var mascota))
            {
                var json = JsonSerializer.Serialize(mascota);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<HttpResponseMessage> PostMascotaAsync(MascotaDTO data)
        {
            await SimulateDelay();

            // Verificar que existe el cliente
            if (!_clientes.ContainsKey(data.NumeroDocumento))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

            var newId = Interlocked.Increment(ref _nextMascotaId);
            data.Id = newId;

            _mascotas.TryAdd(newId, data);

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
        }

        public async Task<HttpResponseMessage> PutMascotaAsync(MascotaDTO data)
        {
            await SimulateDelay();

            if (_mascotas.ContainsKey(data.Id))
            {
                _mascotas.AddOrUpdate(data.Id, data, (key, oldValue) => data);

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<HttpResponseMessage> DeleteMascotaAsync(int id)
        {
            await SimulateDelay();

            if (_mascotas.TryRemove(id, out _))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<HttpResponseMessage> GetMascotasByDuenoAsync(string numeroDocumento)
        {
            await SimulateDelay();

            var mascotasDueno = _mascotas.Values
                .Where(m => m.NumeroDocumento == numeroDocumento)
                .ToList();

            if (_clientes.TryGetValue(numeroDocumento, out var cliente))
            {
                var mascotasConDueno = mascotasDueno.Select(m => new MascotaConDuenoDTO
                {
                    id = m.Id,
                    Nombre = m.Nombre,
                    Especie = m.Especie,
                    Raza = m.Raza,
                    Edad = m.Edad,
                    Genero = m.Genero,
                    Estado = m.Estado,
                    NumeroDocumento = m.NumeroDocumento,
                    PrimerNombreDueno = cliente.PrimerNombre,
                    PrimerApellidoDueno = cliente.PrimerApellido,
                    SegundoApellidoDueno = cliente.SegundoApellido
                }).ToList();

                var json = JsonSerializer.Serialize(mascotasConDueno);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }

        #endregion

        private async Task SimulateDelay()
        {
            // Simular latencia de red y posibles timing issues
            await Task.Delay(_random.Next(1, 10));
        }

        public void Dispose()
        {
            _veterinarios.Clear();
            _clientes.Clear();
            _mascotas.Clear();
        }
    }

    #endregion
}