using System.Text.Json;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;

public static class MedicoDtos {

	public record MedicoDto(
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
		public MedicoDto()
			: this(default, "", "", "", default, "", "", default, "", "", default, null) { }
	}


	public static MedicoDto ToDto(this Medico2025 medico) {
		return new MedicoDto(
			EspecialidadCodigo: medico.EspecialidadUnica.Codigo,
			Dni: medico.Dni.Valor,
			Nombre: medico.NombreCompleto.NombreValor,
			Apellido: medico.NombreCompleto.ApellidoValor,
			FechaIngreso: medico.FechaIngreso,
			Domicilio: medico.Domicilio.DireccionValor,
			Localidad: medico.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo: medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono: medico.Telefono.Valor,
			Email: medico.Email.Valor,
			HaceGuardias: medico.HaceGuardiasValor,
			HorariosJson: JsonSerializer.Serialize(medico.ListaHorarios.ToString()) //Cualquier cosa estaba haciedno aca.
		);
	}

	public static Result<Medico2025> ToDomain(this MedicoDto dto) {
		return Medico2025.CrearResult(
				NombreCompleto2025.CrearResult(dto.Nombre, dto.Apellido),
				//ListaEspecialidadesMedicas2025.CrearConUnicaEspecialidad(
				Especialidad2025.CrearResult(dto.EspecialidadCodigo),
				DniArgentino2025.CrearResult(dto.Dni),
				DomicilioArgentino2025.CrearResult(
					LocalidadDeProvincia2025.CrearResult(
						dto.Localidad,
						ProvinciaArgentina2025.CrearResultPorCodigo(dto.ProvinciaCodigo)),
					dto.Domicilio
				),
				ContactoTelefono2025.CrearResult(dto.Telefono),
				ContactoEmail2025.CrearResult(dto.Email),
				ListaHorarioMedicos2025.CrearResult([.. JsonSerializer.Deserialize<IReadOnlyList<Horario2025>>(dto.HorariosJson)]),
				dto.FechaIngreso,
				dto.HaceGuardias
			);
	}

}
