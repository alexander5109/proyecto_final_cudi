using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;

//public record PacienteListDto(
//	PacienteId Id,
//	string Dni,
//	string Username,
//	string Apellido,
//	string Email,
//	string Telefono
//);
//public record TurnoListDto(
//	TurnoId Id,
//	TimeSpan Hora,
//	DateTime Fecha,
//	EspecialidadEnumCodigo EspecialidadEnumCodigo,
//	TurnoEstadoCodigo Estado,
//	MedicoId MedicoId
//);

//public record MedicoListDto(
//	string Dni,
//	string Username,
//	string Apellido,
//	EspecialidadEnumCodigo EspecialidadEnumCodigo
//);



public static class PacienteDtos {


	public record PacienteDto(
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo ProvinciaCodigo,
		string Telefono,
		string Email,
		DateTime FechaNacimiento
	) {
		public PacienteDto()
			: this("", "", "", default, "", "", default, "", "", default) { }
	}

	public static PacienteDto ToDto(this Paciente2025 paciente) {
		return new PacienteDto(
			Dni: paciente.Dni.Valor,
			Nombre: paciente.NombreCompleto.NombreValor,
			Apellido: paciente.NombreCompleto.ApellidoValor,
			FechaIngreso: paciente.FechaIngreso,
			Domicilio: paciente.Domicilio.DireccionValor,
			Localidad: paciente.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo: paciente.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono: paciente.Telefono.Valor,
			Email: paciente.Email.Valor,
			FechaNacimiento: paciente.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue)
		);
	}



	public static Result<Paciente2025> ToDomain(this PacienteDto dto) {
		return Paciente2025.CrearResult(
			NombreCompleto2025.CrearResult(dto.Nombre, dto.Apellido),
			DniArgentino2025.CrearResult(dto.Dni),
			Telefono2025.CrearResult(dto.Telefono),
			Email2025.CrearResult(dto.Email),
			DomicilioArgentino2025.CrearResult(
			LocalidadDeProvincia2025.CrearResult(
				dto.Localidad,
				ProvinciaArgentina2025.CrearResultPorCodigo(
					dto.ProvinciaCodigo)
				)
			, dto.Domicilio),
			FechaDeNacimiento2025.CrearResult(dto.FechaNacimiento),
			dto.FechaIngreso
		);
	}

}
