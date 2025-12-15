using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Shared.ApiDtos;

public static class HorarioDtos {
	public record HorarioDto(
		MedicoId MedicoId,
		DayOfWeek DiaSemana,
		TimeSpan HoraDesde,
		TimeSpan HoraHasta,
		DateTime VigenteDesde,
		DateTime? VigenteHasta
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

	public static Result<Horario2025> ToDomain(this HorarioDto dto) {
		return Horario2025.CrearResult(
			dto.MedicoId,
			dto.DiaSemana,
			TimeOnly.FromTimeSpan(dto.HoraDesde),
			TimeOnly.FromTimeSpan(dto.HoraHasta),
			new DateOnly(2014, 1, 1),
			new DateOnly(2026, 1, 30)
		);
	}


}
