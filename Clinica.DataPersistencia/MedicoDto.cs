namespace Clinica.DataPersistencia;
public record class HorarioMedicoDto {
	public required int DiaSemana { get; set; }
	public required TimeSpan Desde { get; set; }
	public required TimeSpan Hasta { get; set; }
	public required string? MedicoId { get; set; }
}
public record class MedicoDto {
	public required string Id { get; set; } = string.Empty;
	public required string Name { get; set; } = string.Empty;
	public required string LastName { get; set; } = string.Empty;
	public required string Dni { get; set; } = string.Empty;
	public required string Provincia { get; set; } = string.Empty;
	public required string Domicilio { get; set; } = string.Empty;
	public required string Localidad { get; set; } = string.Empty;
	public required string Especialidad { get; set; } = string.Empty;
	public required string EspecialidadRama { get; set; } = string.Empty;
	public required string Telefono { get; set; } = string.Empty;
	public required bool Guardia { get; set; }
	public required DateTime FechaIngreso { get; set; }
	public required decimal SueldoMinimoGarantizado { get; set; }
	public required List<HorarioMedicoDto> Horarios { get; set; } = [];
}
