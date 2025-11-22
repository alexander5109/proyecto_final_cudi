using System.Text.Json;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.DataPersistencia;

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
public record MedicoDto(
	int Id,
	int EspecialidadCodigoInterno,
	string Dni,
	string Nombre,
	string Apellido,
	DateTime FechaIngreso,
	string Domicilio,
	string Localidad,
	int ProvinciaCodigo,
	string Telefono,
	bool Guardia,
	double SueldoMinimoGarantizado,
	string? HorariosJson    // JSON que contiene la lista de HorarioMedicoDto
) {
	public Result<Medico2025> ToDomain() {
		var json = string.IsNullOrWhiteSpace(HorariosJson) ? "[]" : HorariosJson;
		List<HorarioMedicoDto> horariosDto = JsonSerializer.Deserialize<List<HorarioMedicoDto>>(json) ?? new List<HorarioMedicoDto>();

		return Medico2025.Crear(
			NombreCompleto2025.Crear(Nombre, Apellido),
			ListaEspecialidadesMedicas2025.CrearConUnicaEspecialidad(EspecialidadMedica2025.CrearPorCodigoInterno(EspecialidadCodigoInterno)), // se filtra por EspecialidadCodigoInterno si quieres
			DniArgentino2025.Crear(Dni),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(Localidad, ProvinciaArgentina2025.CrearPorCodigo(ProvinciaCodigo)),
				Domicilio
			),
			ContactoTelefono2025.Crear(Telefono),
			ListaHorarioMedicos2025.Crear(horariosDto.Select(x => x.ToDomain())),
			FechaIngreso2025.Crear(FechaIngreso),
			MedicoSueldoMinimo2025.Crear(SueldoMinimoGarantizado),
			Guardia
		);
	}
}