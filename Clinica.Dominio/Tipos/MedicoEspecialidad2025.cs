using Clinica.Dominio.Comun;
using System.Collections.Immutable;
using System.Linq;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoEspecialidad2025(
	string Titulo,
	string Rama
) {
	public static readonly List<string> EspecialidadesDisponibles = [
		"Clínico General",
		"Cardiólogo",
		"Oftalmólogo",
		"Otorrinolaringólogo",
		"Psiquiatra",
		"Psicólogo",
		"Cirujano",
		"Kinesiólogo",
		"Nutricionista",
		"Gastroenterólogo",
		"Osteópata",
		"Proctólogo",
		"Pediatra",
		"Ginecólogo",
		"Traumatólogo",
		"Neurólogo",
		"Dermatólogo"
	];

	public static readonly List<string> RamasDisponibles = [
		"Clínica Médica",
		"Salud Mental",
		"Cirugía General",
		"Rehabilitación",
		"Cirugía y Ortopedia"
	];

	public static Result<MedicoEspecialidad2025> Crear(string? titulo, string? rama) {
		if (string.IsNullOrWhiteSpace(titulo))
			return new Result<MedicoEspecialidad2025>.Error("El título no puede estar vacío.");
		if (string.IsNullOrWhiteSpace(rama))
			return new Result<MedicoEspecialidad2025>.Error("La rama no puede estar vacía.");
		string candidadoTitulo = titulo.Trim();
		string candidadoRama = rama.Trim();

		if (EspecialidadesDisponibles.Contains(candidadoTitulo) && RamasDisponibles.Contains(candidadoRama)) {
			return new Result<MedicoEspecialidad2025>.Ok(new MedicoEspecialidad2025(candidadoTitulo, candidadoRama));
		} else {
			return new Result<MedicoEspecialidad2025>.Error("La especialidad médica no es válida.");
		}
	}
}
