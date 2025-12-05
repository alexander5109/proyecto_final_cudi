using System.Text.Json;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static partial class DbModels {
	public record MedicoDbModel(
		MedicoId Id,
		EspecialidadCodigo EspecialidadCodigo,
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo2025 ProvinciaCodigo,
		string Telefono,
		string Email,
		bool HaceGuardias,
		string? HorariosJson
	) {
		public MedicoDbModel()
			: this(default!, default, "", "", "", default, "", "", default, "", "", default, null) { }
	}


	public static Result<Medico2025> ToDomain(this MedicoDbModel medicoDto) {
		string json = string.IsNullOrWhiteSpace(medicoDto.HorariosJson) ? "[]" : medicoDto.HorariosJson;
		List<HorarioDbModel> horariosDto = JsonSerializer.Deserialize<List<HorarioDbModel>>(json)
			?? [];
		return Medico2025.Crear(
			MedicoId.Crear(medicoDto.Id.Valor),
			NombreCompleto2025.Crear(medicoDto.Nombre, medicoDto.Apellido),
			//ListaEspecialidadesMedicas2025.CrearConUnicaEspecialidad(
			Especialidad2025.CrearPorCodigoInterno(medicoDto.EspecialidadCodigo),
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


	public static MedicoDbModel ToModel(this Medico2025 medico) {
		return new MedicoDbModel {
			Id = medico.Id,
			EspecialidadCodigo = medico.EspecialidadUnica.Codigo,
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
			HorariosJson = JsonSerializer.Serialize(medico.ToModel())
		};
	}
}
