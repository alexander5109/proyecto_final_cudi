using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;

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

	public static HorarioDbModel ToModel(this Horario2025 horario, HorarioId id) {
		return new HorarioDbModel(
			Id : id,
			MedicoId : horario.MedicoId,
			DiaSemana : horario.DiaSemana,
			HoraDesde : horario.HoraDesde.ToTimeSpan(),
			HoraHasta : horario.HoraHasta.ToTimeSpan(),
			VigenteDesde : horario.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta : horario.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
		);
	}

	public static Result<Horario2025> ToDomain(this HorarioDbModel horarioDto) {
		return Horario2025.CrearResult(
			horarioDto.MedicoId,
			horarioDto.DiaSemana, 
			TimeOnly.FromTimeSpan(horarioDto.HoraDesde),
			TimeOnly.FromTimeSpan(horarioDto.HoraHasta),
			new DateOnly(2014, 1, 1),
			new DateOnly(2026, 1, 1)
		);
	}

}