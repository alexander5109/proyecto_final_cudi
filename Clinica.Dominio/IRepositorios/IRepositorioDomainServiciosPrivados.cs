using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.IRepositorios.QueryModels;

namespace Clinica.Dominio.IRepositorios;

public static class QueryModels {
	public sealed record TurnoQM(
		TurnoId Id,
		MedicoId MedicoId,
		EspecialidadCodigo2025 EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		TurnoOutcomeEstadoCodigo2025 OutcomeEstado // ej: "Programado"
	) {
		public TurnoQM()
			: this(default!, default!, default, default, default, default) { }
	}
	public sealed record HorarioMedicoQM(
		MedicoId MedicoId,
		DayOfWeek DiaSemana,   // 1=Lunes … 7=Domingo
		TimeSpan HoraDesde,
		TimeSpan HoraHasta
	) {
		public HorarioMedicoQM()
			: this(default!, default, default, default) { }
	}
}

public interface IRepositorioDomainServiciosPrivados {
	Task<Result<IEnumerable<TurnoQM>>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<HorarioMedicoQM>>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<MedicoId>>> SelectMedicosIdWhereEspecialidadCode(EspecialidadCodigo2025 code);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 instance); //this 2 can stay cause doesnt ask a model
	Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 instance); //this 2 can stay cause doesnt ask a model
	Task<Result<Usuario2025>> SelectUsuarioWhereIdAsDomain(UsuarioId nombre); //need domain entitiy because this is not really data to query, it's data that immediatly needs domain methods.
	Task<Result<Usuario2025>> SelectUsuarioWhereNombreAsDomain(NombreUsuario nombre); //need domain entitiy because this is not really data to query, it's data that immediatly needs domain methods.
}
