using System.Text.Json.Serialization;
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

public class DiaDeLaSemana {
	public string DiaNombre;
}
public record struct Medico {
	NombreCompleto Nombre;
	EspecialidadMedica Especialidad;
	DniArgentino Dni;
	DomicilioArgentino Domicilio;
	Contacto Contacto;
	MedicoDiasDeAtencion DiasDeAtencion;
}

public record struct CorreoElectronico {
	string Value;
}

public record struct NumeroDeTelefono {
	string Value;
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
	TimeSpan Duracion; // defaults to 40 minutes.
}


public record struct HorarioMedico {
	DiaDeLaSemana DiaSemana;
	TimeOnly Desde;
	TimeOnly Hasta;
}


public record struct MedicoDiasDeAtencion {
	List<HorarioMedico> DiaSemana;
	// asegurar que los timespans no se pisen
}



public record struct DomicilioArgentino {
	LocalidadDeProvincia Localidad;
	ProvinciaDeArgentina Provincia;
	string Direccion;
}

public record struct FechaDeNacimiento {
	DateTime Value;
}


public record struct Contacto {
	CorreoElectronico Email;
	NumeroDeTelefono Telefono;
}

public record struct Paciente {
	NombreCompleto Nombre;
	DniArgentino Dni;
	Contacto Contacto;
	DomicilioArgentino Domicio;
	FechaDeNacimiento FechaNacimiento;
}


