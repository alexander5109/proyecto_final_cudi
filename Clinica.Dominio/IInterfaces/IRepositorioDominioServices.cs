using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Dominio.IInterfaces.QueryModels;

namespace Clinica.Dominio.IInterfaces;

public static class QueryModels {
	public sealed record TurnoQM(
		TurnoId Id,
		MedicoId MedicoId,
		EspecialidadEnum EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		TurnoEstadoEnum OutcomeEstado // ej: "Programado"
	) {
		public TurnoQM()
			: this(default!, default!, default, default, default, default) { }
	}
	public sealed record HorarioMedicoQM(
		MedicoId MedicoId,
		DayOfWeek DiaSemana, 
		TimeSpan HoraDesde,
		TimeSpan HoraHasta
	) {
		public HorarioMedicoQM()
			: this(default!, default, default, default) { }
	}
}

public interface IRepositorioDominioServices {
	Task<Result<IEnumerable<TurnoQM>>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<HorarioMedicoQM>>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta);
	Task<Result<IEnumerable<MedicoId>>> SelectMedicosIdWhereEspecialidadCodigo(EspecialidadEnum code);
	Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 instance); //this 2 can stay cause doesnt ask a model
	Task<Result<Turno2025>> UpdateTurnoWhereIdAndReturnAsDomain(TurnoId id, Turno2025 instance); //this 2 can stay cause doesnt ask a model
	Task<Result<Usuario2025>> SelectUsuarioWhereIdAsDomain(UsuarioId id); //need domain entitiy because this is not really data to query, it's data that immediatly needs domain methods.
    Task<Result<Turno2025>> SelectTurnoWhereIdAsDomain(TurnoId id); //need domain entity for comodidad. dominio debe especializarse en poder proveer esto.
}
