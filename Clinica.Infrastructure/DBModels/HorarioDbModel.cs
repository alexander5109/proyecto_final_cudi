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


	public static HorarioDbModel ToModel(this Horario2025 instance) {
		return new HorarioDbModel {
			Id = instance.Id,
			MedicoId = instance.MedicoId,
			DiaSemana = instance.DiaSemana.Valor,
			HoraDesde = instance.HoraDesde.Valor.ToTimeSpan(),
			HoraHasta = instance.HoraHasta.Valor.ToTimeSpan(),
			VigenteDesde = instance.VigenteDesde.Valor.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta = instance.VigenteHasta.Valor.ToDateTime(TimeOnly.MaxValue)
		};
	}

	public static Result<Horario2025> ToDomain(this HorarioDbModel horarioDto) {
		return Horario2025.Crear(
			horarioDto.Id,
			horarioDto.MedicoId,
			new DiaSemana2025(horarioDto.DiaSemana),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraDesde)),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraHasta)),
			new VigenciaHorario2025(new DateOnly(2014, 1, 1)),
			new VigenciaHorario2025(new DateOnly(2026, 1, 1))
		);
	}

}