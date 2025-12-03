using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.IRepositorios;


public static class QueryModels {
	//public sealed record UsuarioQM( //no hizo falta porque siempre se necesita un usuario2025, no un dto ni querymodel.
	//	UsuarioId UserId,
	//	string UserName,
	//	string UserPassword,
	//	UsuarioEnumRole EnumRole
	//);
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