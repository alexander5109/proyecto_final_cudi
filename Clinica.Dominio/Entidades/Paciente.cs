using Clinica.Dominio.Types;

namespace Clinica.Dominio.Entidades;

public record struct Paciente {
	NombreCompleto Nombre;
	DniArgentino Dni;
	Contacto Contacto;
	DomicilioArgentino Domicio;
	FechaDeNacimiento FechaNacimiento;
}


