
using MediatR;
using System.Text.Json.Serialization;

namespace MIVET.BE.Transversales.Common;


/// <summary>
/// Clase base para todas las entidades de la IPS.
/// Permite manejar eventos de dominio a través de MediatR.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Identificador único de la entidad.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Lista privada de eventos de dominio.
    /// </summary>
    private List<INotification>? _domainEvents;

    /// <summary>
    /// Propiedad de solo lectura que devuelve los eventos de dominio pendientes.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

    /// <summary>
    /// Agrega un evento de dominio a la lista.
    /// </summary>
    /// <param name="event">Evento de dominio a agregar.</param>    
    public void AddDomainEvent(INotification @event)
    {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(@event);
    }

    /// <summary>
    /// Elimina un evento de dominio de la lista.
    /// </summary>
    /// <param name="event">Evento de dominio a eliminar.</param>
    public void RemoveDomainEvent(INotification @event) =>
        _domainEvents?.Remove(@event);

    /// <summary>
    /// Limpia todos los eventos de dominio almacenados.
    /// </summary>
    public void ClearDomainEvents() =>
        _domainEvents?.Clear();
}