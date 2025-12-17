using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Dominio.IInterfaces.QueryModels;

namespace Clinica.Dominio.IInterfaces;

public static class QueryModels {
	public sealed record TurnoQM(
		TurnoId2025 Id,
		MedicoId2025 MedicoId,
		EspecialidadEnum EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		TurnoEstadoEnum OutcomeEstado // ej: "Programado"
	) {
		public TurnoQM()
			: this(default!, default!, default, default, default, default) { }
	}
	public sealed record HorarioMedicoQM(
		MedicoId2025 MedicoId,
		DayOfWeek DiaSemana, 
		TimeSpan HoraDesde,
		TimeSpan HoraHasta
	) {
		public HorarioMedicoQM()
			: this(default!, default, default, default) { }
	}
}

public interface IRepositorioDominioServices {
	Task<Result<IEnumerable<TurnoQM>>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId2025 medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<HorarioMedicoQM>>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId2025 medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<MedicoId2025>>> SelectMedicosIdWhereEspecialidadCodigo(EspecialidadEnum code);
	Task<Result<TurnoId2025>> InsertTurnoReturnId(Turno2025 instance); //this 2 can stay cause doesnt ask a model
	Task<Result<Turno2025>> UpdateTurnoWhereIdAndReturnAsDomain(TurnoId2025 id, Turno2025 instance); //this 2 can stay cause doesnt ask a model
	Task<Result<Usuario2025>> SelectUsuarioWhereIdAsDomain(UsuarioId2025 id); //need domain entitiy because this is not really data to query, it's data that immediatly needs domain methods.
    Task<Result<Turno2025>> SelectTurnoWhereIdAsDomain(TurnoId2025 id); //need domain entity for comodidad. dominio debe especializarse en poder proveer esto.
}
