using System.Text.Json;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Infrastructure.Persistencia;

public record HorarioMedicoDto(
	int Id,
	int MedicoId,
	int DiaSemana,
	TimeOnly HoraDesde,
	TimeOnly HoraHasta
) {
	public Result<HorarioMedico2025> ToDomain() =>
		HorarioMedico2025.Crear(
			new DiaSemana2025((DayOfWeek)DiaSemana),
			new HorarioHora2025(HoraDesde),
			new HorarioHora2025(HoraHasta)
		);
}
public record class MedicoDto {
	public int Id { get; set; }
	public byte EspecialidadCodigoInterno { get; set; }
	public string Dni { get; set; } = "";
	public string Nombre { get; set; } = "";
	public string Apellido { get; set; } = "";
	public DateTime FechaIngreso { get; set; }
	public string Domicilio { get; set; } = "";
	public string Localidad { get; set; } = "";
	public byte ProvinciaCodigo { get; set; }
	public string Telefono { get; set; } = "";
	public string Email { get; set; } = "";
	public bool Guardia { get; set; }
	public string? HorariosJson { get; set; }

	public MedicoDto() { }

	public Result<Medico2025> ToDomain() {
		var json = string.IsNullOrWhiteSpace(HorariosJson) ? "[]" : HorariosJson;
		List<HorarioMedicoDto> horariosDto =
			JsonSerializer.Deserialize<List<HorarioMedicoDto>>(json)
			?? new List<HorarioMedicoDto>();

		return Medico2025.Crear(
			new MedicoId(Id),
			NombreCompleto2025.Crear(Nombre, Apellido),
			ListaEspecialidadesMedicas2025.CrearConUnicaEspecialidad(
				EspecialidadMedica2025.CrearPorCodigoInterno(EspecialidadCodigoInterno)),
			DniArgentino2025.Crear(Dni),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(
					Localidad,
					ProvinciaArgentina2025.CrearPorCodigo(ProvinciaCodigo)),
				Domicilio
			),
			ContactoTelefono2025.Crear(Telefono),
			ListaHorarioMedicos2025.Crear(horariosDto.Select(x => x.ToDomain())),
			FechaIngreso2025.Crear(FechaIngreso),
			Guardia
		);
	}
}
