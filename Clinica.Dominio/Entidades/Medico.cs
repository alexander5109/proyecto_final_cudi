using Clinica.Dominio.Types;

namespace Clinica.Dominio.Entidades;

public record struct Medico (
	NombreCompleto Nombre,
	EspecialidadMedica Especialidad,
	DniArgentino Dni,
	DomicilioArgentino Domicilio,
	Contacto Contacto,
	MedicoDiasDeAtencion DiasDeAtencion
);


