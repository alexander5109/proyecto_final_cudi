using System.Text.RegularExpressions;

namespace Clinica.Dominio;


public record struct NombreCompleto {
	string Nombre;
	string Apellido;
}

public record struct EspecialidadMedica {
	string Titulo;
	string Rama;
}

public record struct ProvinciaDeArgentina {
	string Id;
	string Nombre;
}

public record struct LocalidadDeProvincia {
	string Id;
	string Nombre;
	ProvinciaDeArgentina Provincia;
}

public record struct Domicilio {
	string Direccion;
	LocalidadDeProvincia Localidad;
	string Provincia;
}

public record struct Medico {
	NombreCompleto Nombre;
	EspecialidadMedica Especialidad;
}


public readonly struct DniArgentino {
	private readonly string _value;

	private DniArgentino(string value) => _value = value;

	public static Result<DniArgentino> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DniArgentino>.Error("El DNI no puede estar vacío.");

		var normalized = input.Trim();

		if (!Regex.IsMatch(normalized, @"^\d{1,8}$"))
			return new Result<DniArgentino>.Error("El DNI debe contener hasta 8 dígitos numéricos.");

		return new Result<DniArgentino>.Ok(new DniArgentino(normalized));
	}

	public override string ToString() => _value;

	public static implicit operator string(DniArgentino d) => d._value;
}


public record struct Turno {
	Medico Medico;
	Paciente Paciente;
	DateTime FechaYHora;
}


public record struct Paciente {
	NombreCompleto Nombre;
	DniArgentino Dni;
}
