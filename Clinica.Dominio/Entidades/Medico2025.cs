using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public record struct Medico2025(
	NombreCompleto2025 Nombre,
	EspecialidadMedica2025 Especialidad,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	Contacto2025 Contacto,
	MedicoDiasDeAtencion2025 DiasDeAtencion
);


