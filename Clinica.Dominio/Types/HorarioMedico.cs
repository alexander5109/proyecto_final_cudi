namespace Clinica.Dominio.Types;

public record struct HorarioMedico {
	DiaDeLaSemana DiaSemana;
	TimeOnly Desde;
	TimeOnly Hasta;
}


