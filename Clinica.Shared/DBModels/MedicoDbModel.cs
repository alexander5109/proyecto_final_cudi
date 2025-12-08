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
		return Medico2025.CrearResult(
			MedicoId.CrearResult(medicoDto.Id.Valor),
			NombreCompleto2025.CrearResult(medicoDto.Nombre, medicoDto.Apellido),
			//ListaEspecialidadesMedicas2025.CrearConUnicaEspecialidad(
			Especialidad2025.CrearResult(medicoDto.EspecialidadCodigo),
			DniArgentino2025.CrearResult(medicoDto.Dni),
			DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(
					medicoDto.Localidad,
					ProvinciaArgentina2025.CrearResultPorCodigo(medicoDto.ProvinciaCodigo)),
				medicoDto.Domicilio
			),
			ContactoTelefono2025.CrearResult(medicoDto.Telefono),
			ContactoEmail2025.CrearResult(medicoDto.Email),
			ListaHorarioMedicos2025.CrearResult(horariosDto.Select(x => x.ToDomain())),
			FechaRegistro2025.CrearResult(medicoDto.FechaIngreso),
			medicoDto.HaceGuardias
		);
	}


	public static MedicoDbModel ToModel(this Medico2025Agg aggrg) {
		return new MedicoDbModel {
			Id = aggrg.Id,
			EspecialidadCodigo = aggrg.Medico.EspecialidadUnica.Codigo,
			Dni = aggrg.Medico.Dni.Valor,
			Nombre = aggrg.Medico.NombreCompleto.NombreValor,
			Apellido = aggrg.Medico.NombreCompleto.ApellidoValor,
			FechaIngreso = aggrg.Medico.FechaIngreso.Valor,
			Domicilio = aggrg.Medico.Domicilio.DireccionValor,
			Localidad = aggrg.Medico.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo = aggrg.Medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono = aggrg.Medico.Telefono.Valor,
			Email = aggrg.Medico.Email.Valor,
			HaceGuardias = aggrg.Medico.HaceGuardiasValor,
			//HorariosJson = JsonSerializer.Serialize(aggrg.Medico.ToModel()) //Cualquier cosa estaba haciedno aca.
		};
	}


	public static MedicoDbModel ToModel(this Medico2025 medico) {
		return new MedicoDbModel {
			//Id = aggrg.Id,
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
			//HorariosJson = JsonSerializer.Serialize(aggrg.Medico.ToModel()) //Cualquier cosa estaba haciedno aca.
		};
	}








}
