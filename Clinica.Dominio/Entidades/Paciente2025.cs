using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct Paciente2025EnDb (string Id, Paciente2025 Paciente);


public readonly record struct Paciente2025(
	NombreCompleto2025 NombreCompleto,
	DniArgentino2025 Dni,
	Contacto2025 Contacto,
	DomicilioArgentino2025 Domicilio,
	FechaDeNacimiento2025 FechaNacimiento
) {
	public static Result<Paciente2025> Crear(
	// esto seria el AND
		string nombre,
		string apellido,
		string dni,
		string telefono,
		string email,
		string provincia,
		string localidad,
		string direccion,
		DateOnly fechaNacimiento) {
		// ✅ Encadenar validaciones internas
		var nombreRes = NombreCompleto2025.Crear(nombre, apellido);
		if (nombreRes is Result<NombreCompleto2025>.Error e1) return new Result<Paciente2025>.Error(e1.Mensaje);

		var dniRes = DniArgentino2025.Crear(dni);
		if (dniRes is Result<DniArgentino2025>.Error e2) return new Result<Paciente2025>.Error(e2.Mensaje);

		var contactoRes = Contacto2025.Crear(telefono, email);
		if (contactoRes is Result<Contacto2025>.Error e3) return new Result<Paciente2025>.Error(e3.Mensaje);

		var domicilioRes = DomicilioArgentino2025.Crear(provincia, localidad, direccion);
		if (domicilioRes is Result<DomicilioArgentino2025>.Error e4) return new Result<Paciente2025>.Error(e4.Mensaje);

		var fechaRes = FechaDeNacimiento2025.Crear(fechaNacimiento);
		if (fechaRes is Result<FechaDeNacimiento2025>.Error e5) return new Result<Paciente2025>.Error(e5.Mensaje);

		return new Result<Paciente2025>.Ok(new Paciente2025(
			((Result<NombreCompleto2025>.Ok)nombreRes).Value,
			((Result<DniArgentino2025>.Ok)dniRes).Value,
			((Result<Contacto2025>.Ok)contactoRes).Value,
			((Result<DomicilioArgentino2025>.Ok)domicilioRes).Value,
			((Result<FechaDeNacimiento2025>.Ok)fechaRes).Value
		));
	}

	public override string ToString() => $"{NombreCompleto} ({Dni})";
}
