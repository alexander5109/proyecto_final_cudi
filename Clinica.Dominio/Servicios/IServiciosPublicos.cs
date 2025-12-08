using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Servicios;

public interface IServiciosAuth {
	Task<Result<Usuario2025Agg>> ValidarCredenciales(string username, string password, IRepositorioDomainServiciosPrivados repositorio);
}
public interface IServiciosDisponibilidades {
	Task<Result<IReadOnlyList<Disponibilidad2025>>> SolicitarDisponibilidades(EspecialidadCodigo solicitudEspecialidadCodigo, DateTime aPartirDeCuando, int cuantos, IRepositorioDomainServiciosPrivados repositorio);
}
public interface IServiciosGestionTurnos {
	Task<Result<Turno2025Agg>> PersistirProgramarTurnoAsync(PacienteId pacienteId, DateTime fechaSolicitud, Disponibilidad2025 disponibilidad, IRepositorioDomainServiciosPrivados repositorio);
	Task<Result<Turno2025Agg>> PersistirComoReprogramadoYPersistirProgramarTurnoAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string outcomeComentario, IRepositorioDomainServiciosPrivados repositorio);
	Task<Result<Turno2025Agg>> PersistirComoCanceladoAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string outcomeComentario, IRepositorioDomainServiciosPrivados repositorio);
	Task<Result<Turno2025Agg>> PersistirComoAusenteAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string outcomeComentario, IRepositorioDomainServiciosPrivados repositorio);
	Task<Result<Turno2025Agg>> PersistirComoConcretadoAsync(TurnoId turnoOriginalId, DateTime outcomeFecha, string? outcomeComentario, IRepositorioDomainServiciosPrivados repositorio);
}

public interface IServiciosPublicos : IServiciosAuth, IServiciosDisponibilidades, IServiciosGestionTurnos {
}