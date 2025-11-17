using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.DataPersistencia.ModelDtos;

public record class HorarioMedicoDto {
	public required string DiaSemana { get; set; }
	public required string Desde { get; set; }
	public required string Hasta { get; set; }
	public required int? MedicoId { get; set; }
	public required int? Id { get; set; }



	public static HorarioMedicoDto FromDomain(HorarioMedico2025 horarioMedicoDomain)
		=> new() {
			DiaSemana = horarioMedicoDomain.DiaSemana.Valor.ToString(),
			Desde = horarioMedicoDomain.Desde.Valor.ToString(),
			Hasta = horarioMedicoDomain.Hasta.Valor.ToString(),
			MedicoId = null,
			Id = null,
		};
	public static Result<HorarioMedico2025> ToDomain(HorarioMedicoDto horarioMedicoDto)
		=> HorarioMedico2025.Crear(
			DiaSemana2025.Crear(horarioMedicoDto.DiaSemana),
			HorarioHora2025.Crear(horarioMedicoDto.Desde),
			HorarioHora2025.Crear(horarioMedicoDto.Hasta)
		);

}
