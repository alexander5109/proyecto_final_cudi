using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.Servicios;

public record TurnoCreadoEvent(Turno2025 Turno);
public record TurnoCanceladoEvent(Turno2025 Turno, string? Motivo);
public record TurnoReprogramadoEvent(Turno2025 Turno, DateTime FechaAnterior, DateTime FechaNueva);

public interface IDomainEventDispatcher
{
    void Dispatch(object evt);
}
