using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record HorarioDbModel(
		HorarioId Id,
		MedicoId MedicoId,
		DayOfWeek DiaSemana,
		TimeSpan HoraDesde,
		TimeSpan HoraHasta,
		DateTime VigenteDesde,
		DateTime VigenteHasta
	) {
		public HorarioDbModel()
			: this(default, default, default, default, default, default, default) { }
	}


	public static HorarioDbModel ToModel(this Horario2025Agg aggrg) {
		return new HorarioDbModel {
			Id = aggrg.Id,
			MedicoId = aggrg.Horario.MedicoId,
			DiaSemana = aggrg.Horario.DiaSemana,
			HoraDesde = aggrg.Horario.HoraDesde.ToTimeSpan(),
			HoraHasta = aggrg.Horario.HoraHasta.ToTimeSpan(),
			VigenteDesde = aggrg.Horario.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta = aggrg.Horario.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
		};
	}

	public static Result<Horario2025Agg> ToDomainAgg(this HorarioDbModel dbModel) {
		return Horario2025Agg.CrearResult(
			HorarioId.CrearResult(dbModel.Id),
			Horario2025.CrearResult(
			dbModel.MedicoId,
			dbModel.DiaSemana,
			TimeOnly.FromTimeSpan(dbModel.HoraDesde),
			TimeOnly.FromTimeSpan(dbModel.HoraHasta),
			new DateOnly(2014, 1, 1),
			new DateOnly(2026, 1, 1)
		));
	}

}