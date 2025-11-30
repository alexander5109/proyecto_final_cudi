using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Dtos;

public static partial class DomainDtos {
	public record class PacienteDto {
		public PacienteId Id { get; set; }
		public string Dni { get; set; } = "";
		public string? Nombre { get; set; }
		public string? Apellido { get; set; }
		public DateTime FechaIngreso { get; set; }
		public string? Domicilio { get; set; }
		public string? Localidad { get; set; }
		public ProvinciaCodigo2025 ProvinciaCodigo { get; set; }
		public string Telefono { get; set; } = "";
		public string? Email { get; set; }
		public DateTime? FechaNacimiento { get; set; }

		public PacienteDto() { }

	}
	public static Result<Paciente2025> ToDomain(this PacienteDto pacientedto) {
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

	public static PacienteDto ToDto(this Paciente2025 paciente) {
		return new PacienteDto {
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
			FechaNacimiento = paciente.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue)
		};
	}
}