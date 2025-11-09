using Clinica.Dominio.Comun;
using Clinica.Dominio.Types;

namespace Clinica.Dominio.Entidades;

public record Paciente(
	NombreCompleto Nombre,
	DniArgentino Dni,
	Contacto Contacto,
	DomicilioArgentino Domicilio,
	FechaDeNacimiento FechaNacimiento
);
