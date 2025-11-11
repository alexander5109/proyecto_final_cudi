using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;
using Microsoft.VisualBasic.FileIO;
using System;

namespace Clinica.Dominio.Entidades;

public readonly record struct Paciente2025EnDb (string Id, Paciente2025 Paciente);

public readonly record struct Paciente2025(
	Option<NombreCompleto2025> NombreCompleto,
	Option<DniArgentino2025> Dni,
	Option<Contacto2025> Contacto,
	Option<DomicilioArgentino2025> Domicilio,
	Option<FechaDeNacimiento2025> FechaNacimiento
) {
	// ✅ Nuevo constructor: valida todo y acumula errores
	public static Result<Paciente2025> Crear(
		string nombre,
		string apellido,
		string dni,
		string telefono,
		string email,
		string provincia,
		string localidad,
		string direccion,
		DateOnly fechaNacimiento) {
		var nombreRes = NombreCompleto2025.Crear(nombre, apellido);
		var dniRes = DniArgentino2025.Crear(dni);
		var contactoRes = Contacto2025.Crear(telefono, email);
		var domicilioRes = DomicilioArgentino2025.Crear(provincia, localidad, direccion);
		var fechaRes = FechaDeNacimiento2025.Crear(fechaNacimiento);

		// 👇 Combina todas las validaciones (no aborta en el primer error)
		var combinado = ResultExtensions.Combine(nombreRes, dniRes, contactoRes, domicilioRes, fechaRes);

		// Si hay errores, devolvemos Result.Error pero aún así instanciamos un "paciente incompleto"
		return combinado switch {
			Result<(NombreCompleto2025, DniArgentino2025, Contacto2025, DomicilioArgentino2025, FechaDeNacimiento2025)>.Ok ok
				=> new Result<Paciente2025>.Ok(new Paciente2025(
					Option.Some(ok.Value.Item1),
					Option.Some(ok.Value.Item2),
					Option.Some(ok.Value.Item3),
					Option.Some(ok.Value.Item4),
					Option.Some(ok.Value.Item5)
				)),

			Result<(NombreCompleto2025, DniArgentino2025, Contacto2025, DomicilioArgentino2025, FechaDeNacimiento2025)>.Error e
				=> new Result<Paciente2025>.Error(e.Mensaje)
		};
	}

	public override string ToString() {
		string nombre = NombreCompleto.Match(n => n.ToString(), () => "(sin nombre)");
		string dni = Dni.Match(d => d.ToString(), () => "(sin DNI)");
		return $"{nombre} [{dni}]";
	}
}
