namespace Clinica.Shared.ApiDtos;

public static class HorarioDtos {
	public record HorarioDto(
		//MedicoId2025 MedicoId2025,
		DayOfWeek DiaSemana,
		TimeSpan HoraDesde,
		TimeSpan HoraHasta,
		DateTime VigenteDesde,
		DateTime? VigenteHasta
	) {
		public HorarioDto()
			: this(default, default, default, default, default) { }
	}
	//public static HorarioDto ToDto(this Horario2025 instance) {
	//	return new HorarioDto {
	//		MedicoId2025 = instance.MedicoId2025,
	//		DiaSemana = instance.DiaSemana,
	//		HoraDesde = instance.HoraDesde.ToTimeSpan(),
	//		HoraHasta = instance.HoraHasta.ToTimeSpan(),
	//		VigenteDesde = instance.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
	//		VigenteHasta = instance.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
	//	};
	//}

	//public static Result<Horario2025> ToDomain(this HorarioDto dto) {
	//	return Horario2025.CrearResult(
	//		dto.MedicoId2025,
	//		dto.DiaSemana,
	//		TimeOnly.FromTimeSpan(dto.HoraDesde),
	//		TimeOnly.FromTimeSpan(dto.HoraHasta),
	//		new DateOnly(2014, 1, 1),
	//		new DateOnly(2026, 1, 30)
	//	);
	//}


}
