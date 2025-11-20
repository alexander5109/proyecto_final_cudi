using Clinica.Dominio.FunctionalProgramingTools;

namespace Clinica.Dominio.Entidades;


public readonly record struct TardeOMañana(bool Tarde);

public static partial class Entidades {
	public readonly record struct SolicitudDeTurno(
		Paciente2025 Paciente,
		EspecialidadMedica2025 Especialidad,
		DiaSemana2025 DiaPreferido,
		TardeOMañana PrefiereTardeOMañana,
		DateTime SolicitudEn
	);
}