namespace Clinica.Dominio.Types;

public record struct MedicoDiasDeAtencion {
	List<HorarioMedico> DiaSemana;
	// asegurar que los timespans no se pisen
}


