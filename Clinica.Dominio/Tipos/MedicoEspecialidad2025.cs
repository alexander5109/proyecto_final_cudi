using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoEspecialidad2025(
	string Titulo,
	string Rama
){
	// Conjunto inmutable de especialidades válidas
	public static readonly Dictionary<int, MedicoEspecialidad2025> EspecialidadesValidas = new Dictionary<int, MedicoEspecialidad2025>{
		{ 1, new("Clínico General", "Clínica Médica") },
		{ 2, new("Cardiólogo", "Clínica Médica") },
		{ 3, new("Oftalmólogo", "Clínica Médica") },
		{ 4, new("Otorrinolaringólogo", "Clínica Médica") },
		{ 5, new("Psiquiatra", "Salud Mental") },
		{ 6, new("Psicólogo", "Salud Mental") },
		{ 7, new("Cirujano", "Cirugía General") },
		{ 8, new("Kinesiólogo", "Rehabilitación") },
		{ 9, new("Nutricionista", "Salud General") },
		{ 10, new ("Abortera Clandestina", "Cirugía General") },
		{ 11, new ("Curadora de Empachos", "Clínica Médica") },
		{ 12, new ("Gastroenterólogo", "Clínica Médica") },
		{ 13, new ("Masagista de Genitales", "Salud Mental") },
		{ 14, new ("Osteópata", "Clínica Médica") },
		{ 15, new ("Proctologo", "Clínica Médica") },
		{ 16, new ("Traficante de Estupefacientes", "Rehabilitación") },
		{ 17, new("Pediatra", "Clínica Médica") },
		{ 18, new("Ginecólogo", "Clínica Médica") },
		{ 19, new("Traumatólogo", "Cirugía y Ortopedia") },
		{ 20, new("Neurólogo", "Clínica Médica") },
		{ 21, new("Dermatólogo", "Clínica Médica") },
	};

	public static Result<MedicoEspecialidad2025> Crear(string titulo, string rama) {
		if (string.IsNullOrWhiteSpace(titulo))
			return new Result<MedicoEspecialidad2025>.Error("El título no puede estar vacío.");
		if (string.IsNullOrWhiteSpace(rama))
			return new Result<MedicoEspecialidad2025>.Error("La rama no puede estar vacía.");

		var candidato = new MedicoEspecialidad2025(titulo.Trim(), rama.Trim());
		if (!EspecialidadesValidas.Values.Contains(candidato))
			return new Result<MedicoEspecialidad2025>.Error("La especialidad médica no es válida.");

		return new Result<MedicoEspecialidad2025>.Ok(candidato);
	}
}
