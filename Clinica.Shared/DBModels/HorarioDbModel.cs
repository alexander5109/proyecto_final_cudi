using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

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
			DiaSemana = aggrg.Horario.DiaSemana.EnumValor,
			HoraDesde = aggrg.Horario.HoraDesde.Valor.ToTimeSpan(),
			HoraHasta = aggrg.Horario.HoraHasta.Valor.ToTimeSpan(),
			VigenteDesde = aggrg.Horario.VigenteDesde.Valor.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta = aggrg.Horario.VigenteHasta.Valor.ToDateTime(TimeOnly.MaxValue)
		};
	}

	public static HorarioDbModel ToModel(this Horario2025 horario, HorarioId id) {
		return new HorarioDbModel(
			Id : id,
			MedicoId : horario.MedicoId,
			DiaSemana : horario.DiaSemana.EnumValor,
			HoraDesde : horario.HoraDesde.Valor.ToTimeSpan(),
			HoraHasta : horario.HoraHasta.Valor.ToTimeSpan(),
			VigenteDesde : horario.VigenteDesde.Valor.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta : horario.VigenteHasta.Valor.ToDateTime(TimeOnly.MaxValue)
		);
	}

	public static Result<Horario2025> ToDomain(this HorarioDbModel horarioDto) {
		return Horario2025.CrearResult(
			//horarioDto.Id,
			horarioDto.MedicoId,
			new DiaSemana2025(horarioDto.DiaSemana, horarioDto.DiaSemana.AEspañol()),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraDesde)),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraHasta)),
			new VigenciaHorario2025(new DateOnly(2014, 1, 1)),
			new VigenciaHorario2025(new DateOnly(2026, 1, 1))
		);
	}

}