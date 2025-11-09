namespace Clinica.Dominio.Tipos;

public record struct HorarioMedico2025 {
	DiaDeLaSemana2025 DiaSemana;
	TimeOnly Desde;
	TimeOnly Hasta;
}


