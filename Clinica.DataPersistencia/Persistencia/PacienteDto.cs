using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Infrastructure.Persistencia;

public record class PacienteDto {
	public int Id { get; set; }
	public string Dni { get; set; } = "";
	public string? Nombre { get; set; }
	public string? Apellido { get; set; }
	public DateTime FechaIngreso { get; set; }
	public string? Domicilio { get; set; }
	public string? Localidad { get; set; }
	public byte? ProvinciaCodigo { get; set; }
	public string Telefono { get; set; } = "";
	public string? Email { get; set; }
	public DateTime? FechaNacimiento { get; set; }

	public PacienteDto() { }

	public Result<Paciente2025> ToDomain() {
        Result<NombreCompleto2025> nombre = NombreCompleto2025.Crear(Nombre ?? "", Apellido ?? "");
        Result<DniArgentino2025> dni = DniArgentino2025.Crear(Dni);

        Result<Contacto2025> contacto = Contacto2025.Crear(
			ContactoEmail2025.Crear(Email ?? ""),
			ContactoTelefono2025.Crear(Telefono)
		);

        Result<DomicilioArgentino2025> domicilio = DomicilioArgentino2025.Crear(
			LocalidadDeProvincia2025.Crear(
				Localidad ?? "",
				ProvinciaArgentina2025.CrearPorCodigo(ProvinciaCodigo)
			),
			Domicilio ?? ""
		);

        Result<FechaDeNacimiento2025> fechaNacimiento =
			FechaNacimiento is null
				? FechaDeNacimiento2025.Crear(DateOnly.FromDateTime(DateTime.UnixEpoch))
				: FechaDeNacimiento2025.Crear(DateOnly.FromDateTime(FechaNacimiento.Value));

        Result<FechaIngreso2025> ingreso = FechaIngreso2025.Crear(FechaIngreso);

		return Paciente2025.Crear(
			new PacienteId(Id),
			nombre,
			dni,
			contacto,
			domicilio,
			fechaNacimiento,
			ingreso
		);
	}

	public static PacienteDto FromDomain(Paciente2025 p, int id = 0) {
		return new PacienteDto {
			Id = id,
			Dni = p.Dni.Valor,
			Nombre = p.NombreCompleto.Nombre,
			Apellido = p.NombreCompleto.Apellido,
			FechaIngreso = p.FechaIngreso.Valor,
			Domicilio = p.Domicilio.Direccion,
			Localidad = p.Domicilio.Localidad.Nombre,
			ProvinciaCodigo = (byte)p.Domicilio.Localidad.Provincia.CodigoInterno,
			Telefono = p.Contacto.Telefono.Valor,
			Email = p.Contacto.Email.Valor,
			FechaNacimiento = p.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue)
		};
	}
}
