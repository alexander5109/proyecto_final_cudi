using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;


public static class PacienteDtos {


	public record PacienteDto(
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo2025 ProvinciaCodigo,
		string Telefono,
		string Email,
		DateTime FechaNacimiento
	) {
		public PacienteDto()
			: this("", "", "", default, "", "", default, "", "", default) { }
	}
	public static Result<Paciente2025> ToDomain(this PacienteDto pacientedto) {
		return Paciente2025.CrearResult(
			NombreCompleto2025.CrearResult(pacientedto.Nombre, pacientedto.Apellido),
			DniArgentino2025.CrearResult(pacientedto.Dni),
			Contacto2025.CrearResult(
			ContactoEmail2025.CrearResult(pacientedto.Email),
			ContactoTelefono2025.CrearResult(pacientedto.Telefono)),
			DomicilioArgentino2025.CrearResult(
			LocalidadDeProvincia2025.CrearResult(
				pacientedto.Localidad,
				ProvinciaArgentina2025.CrearResultPorCodigo(
					pacientedto.ProvinciaCodigo)
				)
			, pacientedto.Domicilio),
			FechaDeNacimiento2025.CrearResult(pacientedto.FechaNacimiento),
			pacientedto.FechaIngreso
		);
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
			Telefono: paciente.Contacto.Telefono.Valor,
			Email: paciente.Contacto.Email.Valor,
			FechaNacimiento: paciente.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue)
		);
	}



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
	//	EspecialidadCodigo EspecialidadCodigo,
	//	TurnoEstadoCodigo Estado,
	//	MedicoId MedicoId
	//);

	//public record MedicoListDto(
	//	string Dni,
	//	string Username,
	//	string Apellido,
	//	EspecialidadCodigo EspecialidadCodigo
	//);



}
