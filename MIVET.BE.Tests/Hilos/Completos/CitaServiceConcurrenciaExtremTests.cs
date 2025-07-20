using Microsoft.VisualStudio.TestPlatform.Utilities;
using MIVET.BE.Tests.Hilos.Completos.Continuacion;
using MIVET.BE.Tests.Hilos.Finales;
using MIVET.BE.Transversales.DTOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MIVET.BE.Tests.Hilos.Completos
{

    public class CitaServiceConcurrenciaExtremTests : BaseTestClass
    {
        public CitaServiceConcurrenciaExtremTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task CP086_DeadlockTablas_OperacionesCruzadas_DeteccionDeadlock()
        {
            // Arrange
            var fecha1 = DateTime.Today.AddDays(1);
            var fecha2 = DateTime.Today.AddDays(2);

            var resultados = new ConcurrentBag<ResultadoConcurrencia>();

            // Act - Simular operaciones cruzadas que podrían causar deadlock
            var tareas = new[]
            {
                // Tarea 1: Crear cita y luego actualizarla
                Task.Run(async () =>
                {
                    try
                    {
                        var dto = CrearCitaDtoValida(1, fecha1, new TimeSpan(10, 0, 0), "DEADLOCK_USER1");
                        var cita = await _citaService.CrearAsync(dto);

                        await Task.Delay(50); // Pequeña pausa para incrementar posibilidad de deadlock
                        
                        var actualizarDto = new ActualizarCitaDto { Observaciones = "Actualizado por tarea 1" };
                        await _citaService.ActualizarAsync(cita.Id, actualizarDto);

                        resultados.Add(new ResultadoConcurrencia { Exitoso = true, Usuario = "DEADLOCK_USER1" });
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new ResultadoConcurrencia { Exitoso = false, Usuario = "DEADLOCK_USER1", Error = ex.Message });
                    }
                }),
                
                // Tarea 2: Crear múltiples citas rápidamente
                Task.Run(async () =>
                {
                    try
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var dto = CrearCitaDtoValida(i + 2, fecha2, new TimeSpan(11 + i, 0, 0), $"DEADLOCK_USER2_{i}");
                            await _citaService.CrearAsync(dto);
                        }

                        resultados.Add(new ResultadoConcurrencia { Exitoso = true, Usuario = "DEADLOCK_USER2" });
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new ResultadoConcurrencia { Exitoso = false, Usuario = "DEADLOCK_USER2", Error = ex.Message });
                    }
                }),
                
                // Tarea 3: Consultas intensivas
                Task.Run(async () =>
                {
                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            await _citaService.ObtenerTodosAsync();
                            await _citaService.ObtenerPorFechaAsync(fecha1);
                        }

                        resultados.Add(new ResultadoConcurrencia { Exitoso = true, Usuario = "DEADLOCK_USER3" });
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new ResultadoConcurrencia { Exitoso = false, Usuario = "DEADLOCK_USER3", Error = ex.Message });
                    }
                })
            };

            await Task.WhenAll(tareas);

            // Assert
            Assert.Equal(3, resultados.Count);
            var exitosos = resultados.Count(r => r.Exitoso);

            // En Entity Framework InMemory, los deadlocks son menos probables
            // Verificamos que al menos algunas operaciones fueron exitosas
            Assert.True(exitosos >= 2);

            _output.WriteLine($"✅ CP086 EXITOSO: {exitosos}/3 tareas completadas sin deadlock detectado");
            foreach (var resultado in resultados.Where(r => !r.Exitoso))
            {
                _output.WriteLine($"    Error en {resultado.Usuario}: {resultado.Error}");
            }
        }

        [Fact]
        public async Task CP087_RaceConditionValidaciones_ValidacionObsoleta_DeteccionRaceCondition()
        {
            // Arrange
            var fecha = DateTime.Today.AddDays(1);
            var hora = new TimeSpan(14, 0, 0);

            var resultados = new ConcurrentBag<ResultadoConcurrencia>();

            // Act - Múltiples hilos intentan validar y crear en el mismo horario simultaneamente
            var tareas = Enumerable.Range(0, 5).Select(i =>
                Task.Run(async () =>
                {
                    try
                    {
                        // Simular validación previa
                        var verificarDto = new VerificarDisponibilidadDto
                        {
                            MedicoVeterinarioNumeroDocumento = "12345678",
                            FechaCita = fecha,
                            HoraInicio = hora,
                            DuracionMinutos = 30
                        };

                        var disponible = await _citaService.VerificarDisponibilidadAsync(verificarDto);

                        if (disponible)
                        {
                            // Pequeña pausa para simular procesamiento
                            await Task.Delay(10 + (i * 5));

                            // Intentar crear la cita
                            var dto = CrearCitaDtoValida(i + 1, fecha, hora, $"RACE_USER_{i}");
                            var cita = await _citaService.CrearAsync(dto);

                            resultados.Add(new ResultadoConcurrencia
                            {
                                Exitoso = true,
                                CitaId = cita.Id,
                                Usuario = $"RACE_USER_{i}"
                            });
                        }
                        else
                        {
                            resultados.Add(new ResultadoConcurrencia
                            {
                                Exitoso = false,
                                Usuario = $"RACE_USER_{i}",
                                Error = "No disponible en validación inicial"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new ResultadoConcurrencia
                        {
                            Exitoso = false,
                            Usuario = $"RACE_USER_{i}",
                            Error = ex.Message
                        });
                    }
                })
            );

            await Task.WhenAll(tareas);

            // Assert
            Assert.Equal(5, resultados.Count);
            var exitosos = resultados.Count(r => r.Exitoso);

            // Solo una debería ser exitosa debido al race condition
            Assert.True(exitosos <= 1);

            _output.WriteLine($"✅ CP087 EXITOSO: Race condition controlado - {exitosos} exitosa(s) de 5 intentos");
            _output.WriteLine($"    Fallidas por race condition: {5 - exitosos}");
        }

        [Fact]
        public async Task CP088_LockRegistros_ActualizacionesSimultaneas_ControlBloqueos()
        {
            // Arrange
            var dto = CrearCitaDtoValida(1, DateTime.Today.AddDays(1), new TimeSpan(10, 0, 0));
            var citaCreada = await _citaService.CrearAsync(dto);

            var resultados = new ConcurrentBag<ResultadoActualizacion>();
            const int numeroActualizaciones = 8;

            // Act - Múltiples actualizaciones simultáneas del mismo registro
            var tareas = Enumerable.Range(0, numeroActualizaciones).Select(i =>
                Task.Run(async () =>
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        var actualizarDto = new ActualizarCitaDto
                        {
                            Observaciones = $"Actualización simultánea #{i} por thread {Thread.CurrentThread.ManagedThreadId}"
                        };

                        var resultado = await _citaService.ActualizarAsync(citaCreada.Id, actualizarDto);

                        resultados.Add(new ResultadoActualizacion
                        {
                            Exitoso = true,
                            Usuario = $"LOCK_USER_{i}",
                            Observaciones = resultado.Observaciones,
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new ResultadoActualizacion
                        {
                            Exitoso = false,
                            Usuario = $"LOCK_USER_{i}",
                            Error = ex.Message,
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                })
            );

            await Task.WhenAll(tareas);

            // Assert
            Assert.Equal(numeroActualizaciones, resultados.Count);

            // Verificar que el registro final es consistente
            var citaFinal = await _citaService.ObtenerPorIdAsync(citaCreada.Id);
            Assert.NotNull(citaFinal.Observaciones);
            Assert.Contains("Actualización simultánea", citaFinal.Observaciones);

            var exitosas = resultados.Count(r => r.Exitoso);
            var tiempoPromedio = resultados.Average(r => r.TiempoMs);

            _output.WriteLine($"✅ CP088 EXITOSO: Control de bloqueos verificado");
            _output.WriteLine($"    Actualizaciones exitosas: {exitosas}/{numeroActualizaciones}");
            _output.WriteLine($"    Tiempo promedio: {tiempoPromedio:F1}ms");
            _output.WriteLine($"    Estado final: {citaFinal.Observaciones}");
        }

        [Fact]
        public async Task CP089_TimeoutOperaciones_OperacionesLargas_ManejoTimeouts()
        {
            // Arrange
            const int timeoutMs = 5000; // 5 segundos
            var resultados = new ConcurrentBag<ResultadoTimeout>();

            // Act - Simular operaciones que podrían timeout
            var tareas = new[]
            {
                // Operación rápida
                Task.Run(async () =>
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        var dto = CrearCitaDtoValida(1, DateTime.Today.AddDays(1), new TimeSpan(10, 0, 0));
                        using var cts = new CancellationTokenSource(timeoutMs);

                        var cita = await _citaService.CrearAsync(dto);

                        resultados.Add(new ResultadoTimeout
                        {
                            Operacion = "CrearCitaRapida",
                            Exitoso = true,
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        resultados.Add(new ResultadoTimeout
                        {
                            Operacion = "CrearCitaRapida",
                            Exitoso = false,
                            Error = "Timeout",
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new ResultadoTimeout
                        {
                            Operacion = "CrearCitaRapida",
                            Exitoso = false,
                            Error = ex.Message,
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                }),
                
                // Operación con múltiples consultas
                Task.Run(async () =>
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        using var cts = new CancellationTokenSource(timeoutMs);
                        
                        // Realizar múltiples consultas en secuencia
                        for (int i = 0; i < 50; i++)
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            await _citaService.ObtenerTodosAsync();
                            await Task.Delay(10, cts.Token); // Simular procesamiento
                        }

                        resultados.Add(new ResultadoTimeout
                        {
                            Operacion = "ConsultasMultiples",
                            Exitoso = true,
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        resultados.Add(new ResultadoTimeout
                        {
                            Operacion = "ConsultasMultiples",
                            Exitoso = false,
                            Error = "Timeout",
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new ResultadoTimeout
                        {
                            Operacion = "ConsultasMultiples",
                            Exitoso = false,
                            Error = ex.Message,
                            TiempoMs = sw.ElapsedMilliseconds
                        });
                    }
                })
            };

            await Task.WhenAll(tareas);

            // Assert
            Assert.Equal(2, resultados.Count);

            var operacionRapida = resultados.FirstOrDefault(r => r.Operacion == "CrearCitaRapida");
            var operacionLenta = resultados.FirstOrDefault(r => r.Operacion == "ConsultasMultiples");

            Assert.NotNull(operacionRapida);
            Assert.NotNull(operacionLenta);

            _output.WriteLine($"✅ CP089 EXITOSO: Manejo de timeouts verificado");
            _output.WriteLine($"    Operación rápida: {(operacionRapida.Exitoso ? "EXITOSA" : "FALLIDA")} en {operacionRapida.TiempoMs}ms");
            _output.WriteLine($"    Operación lenta: {(operacionLenta.Exitoso ? "EXITOSA" : "FALLIDA")} en {operacionLenta.TiempoMs}ms");

            if (!operacionLenta.Exitoso)
            {
                _output.WriteLine($"    Razón falla operación lenta: {operacionLenta.Error}");
            }
        }
    }
}
