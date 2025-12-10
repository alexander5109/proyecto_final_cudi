using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Shared.ApiDtos;

public static class HorarioDtos {
	public record HorarioDto(
		MedicoId MedicoId,
		DayOfWeek DiaSemana,
		TimeSpan HoraDesde,
		TimeSpan HoraHasta,
		DateTime VigenteDesde,
		DateTime VigenteHasta
	) {
		public HorarioDto()
			: this(default, default, default, default, default, default) { }
	}
	public static HorarioDto ToDto(this Horario2025 instance) {
		return new HorarioDto {
			MedicoId = instance.MedicoId,
			DiaSemana = instance.DiaSemana,
			HoraDesde = instance.HoraDesde.ToTimeSpan(),
			HoraHasta = instance.HoraHasta.ToTimeSpan(),
			VigenteDesde = instance.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta = instance.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
		};
	}

	public static Result<Horario2025> ToDomain(this HorarioDto horarioDto) {
		return Horario2025.CrearResult(
			horarioDto.MedicoId,
			horarioDto.DiaSemana,
			TimeOnly.FromTimeSpan(horarioDto.HoraDesde),
			TimeOnly.FromTimeSpan(horarioDto.HoraHasta),
			new DateOnly(2014, 1, 1),
			new DateOnly(2026, 1, 30)
		);
	}


}
