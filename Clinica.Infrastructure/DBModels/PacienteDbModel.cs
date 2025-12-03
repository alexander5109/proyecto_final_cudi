using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static partial class DbModels {
	public record PacienteDbModel(
		PacienteId Id,
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
		public PacienteDbModel()
			: this(default!, "", "", "", default, "", "", default, "", "", default) { }
	}
	public static Result<Paciente2025> ToDomain(this PacienteDbModel pacientedto) {
		return Paciente2025.Crear(
			PacienteId.Crear(pacientedto.Id.Valor),
			NombreCompleto2025.Crear(pacientedto.Nombre, pacientedto.Apellido),
			DniArgentino2025.Crear(pacientedto.Dni),
			Contacto2025.Crear(
			ContactoEmail2025.Crear(pacientedto.Email),
			ContactoTelefono2025.Crear(pacientedto.Telefono)),
			DomicilioArgentino2025.Crear(
			LocalidadDeProvincia2025.Crear(
				pacientedto.Localidad,
				ProvinciaArgentina2025.CrearPorCodigo(
					pacientedto.ProvinciaCodigo)
				)
			, pacientedto.Domicilio),
			FechaDeNacimiento2025.Crear(pacientedto.FechaNacimiento),
			FechaRegistro2025.Crear(pacientedto.FechaIngreso)
		);
	}

	public static PacienteDbModel ToModel(this Paciente2025 paciente) {
		return new PacienteDbModel {
			Id = paciente.Id,
			Dni = paciente.Dni.Valor,
			Nombre = paciente.NombreCompleto.NombreValor,
			Apellido = paciente.NombreCompleto.ApellidoValor,
			FechaIngreso = paciente.FechaIngreso.Valor,
			Domicilio = paciente.Domicilio.DireccionValor,
			Localidad = paciente.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo = paciente.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono = paciente.Contacto.Telefono.Valor,
			Email = paciente.Contacto.Email.Valor,
			FechaNacimiento = paciente.FechaNacimiento.Valor.ToDateTime(TimeOnly.MaxValue),
		};
	}
}