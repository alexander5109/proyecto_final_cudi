using Clinica.Dominio.Comun;
using System.Collections.Immutable;
using System.Linq;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct MedicoEspecialidad2025(
	string Titulo,
	string Rama,
	int DuracionConsultaMinutos
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

	// Duraciones por defecto por especialidad (minutos)
	private static readonly Dictionary<string, int> DuracionesPorEspecialidad = new(StringComparer.OrdinalIgnoreCase) {
		["Clínico General"] = 30,
		["Cardiólogo"] = 40,
		["Oftalmólogo"] = 20,
		["Otorrinolaringólogo"] = 25,
		["Psiquiatra"] = 50,
		["Psicólogo"] = 50,
		["Cirujano"] = 60,
		["Kinesiólogo"] = 30,
		["Nutricionista"] = 30,
		["Gastroenterólogo"] = 40,
		["Osteópata"] = 30,
		["Proctólogo"] = 30,
		["Pediatra"] = 25,
		["Ginecólogo"] = 35,
		["Traumatólogo"] = 40,
		["Neurólogo"] = 45,
		["Dermatólogo"] = 20
	};

	public static Result<MedicoEspecialidad2025> Crear(string? titulo, string? rama, int? duracionMinutos = null) {
		if (string.IsNullOrWhiteSpace(titulo))
			return new Result<MedicoEspecialidad2025>.Error("El título no puede estar vacío.");
		if (string.IsNullOrWhiteSpace(rama))
			return new Result<MedicoEspecialidad2025>.Error("La rama no puede estar vacía.");
		string candidadoTitulo = titulo.Trim();
		string candidadoRama = rama.Trim();

		if (EspecialidadesDisponibles.Contains(candidadoTitulo) && RamasDisponibles.Contains(candidadoRama)) {
			int dur = duracionMinutos ?? (DuracionesPorEspecialidad.ContainsKey(candidadoTitulo) ? DuracionesPorEspecialidad[candidadoTitulo] : 30);
			return new Result<MedicoEspecialidad2025>.Ok(new MedicoEspecialidad2025(candidadoTitulo, candidadoRama, dur));
		} else {
			return new Result<MedicoEspecialidad2025>.Error("La especialidad médica no es válida.");
		}
	}
}
