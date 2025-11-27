using System.Runtime.CompilerServices;
using System.Text.Json;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Infrastructure.DtosEntidades;

public static partial class DtosEntidades {
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
		public bool HaceGuardias { get; set; }
		public string? HorariosJson { get; set; }
		public MedicoDto() { }
	}

	public static Result<Medico2025> ToDomain(this MedicoDto medicoDto) {
		string json = string.IsNullOrWhiteSpace(medicoDto.HorariosJson) ? "[]" : medicoDto.HorariosJson;
		List<HorarioMedicoDto> horariosDto = JsonSerializer.Deserialize<List<HorarioMedicoDto>>(json)
			?? [];
		return Medico2025.Crear(
			new MedicoId(medicoDto.Id),
			NombreCompleto2025.Crear(medicoDto.Nombre, medicoDto.Apellido),
			//ListaEspecialidadesMedicas2025.CrearConUnicaEspecialidad(
			EspecialidadMedica2025.CrearPorCodigoInterno(medicoDto.EspecialidadCodigoInterno),
			DniArgentino2025.Crear(medicoDto.Dni),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(
					medicoDto.Localidad,
					ProvinciaArgentina2025.CrearPorCodigo(medicoDto.ProvinciaCodigo)),
				medicoDto.Domicilio
			),
			ContactoTelefono2025.Crear(medicoDto.Telefono),
			ContactoEmail2025.Crear(medicoDto.Email),
			ListaHorarioMedicos2025.Crear(horariosDto.Select(x => x.ToDomain())),
			FechaRegistro2025.Crear(medicoDto.FechaIngreso),
			medicoDto.HaceGuardias
		);
	}


	public static MedicoDto ToDto(this Medico2025 medico) {
		return new MedicoDto {
			Id = medico.Id.Valor,
			EspecialidadCodigoInterno = medico.EspecialidadUnica.CodigoInterno.Valor,
			Dni = medico.Dni.Valor,
			Nombre = medico.NombreCompleto.NombreValor,
			Apellido = medico.NombreCompleto.ApellidoValor,
			FechaIngreso = medico.FechaIngreso.Valor,
			Domicilio = medico.Domicilio.DireccionValor,
			Localidad = medico.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo = medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono = medico.Telefono.Valor,
			Email = medico.Email.Valor,
			HaceGuardias = medico.HaceGuardiasValor,
			HorariosJson = JsonSerializer.Serialize(medico.ToDto())
		};
	}
}
