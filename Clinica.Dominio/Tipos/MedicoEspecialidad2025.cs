using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoEspecialidad2025(
	string Titulo, 
	string Rama
) {
	// Conjunto inmutable de especialidades válidas
	public static readonly IImmutableSet<MedicoEspecialidad2025> EspecialidadesValidas =
		new HashSet<MedicoEspecialidad2025>
		{
			new("Clínico General", "Clínica Médica"),
			new("Cardiólogo", "Clínica Médica"),
			new("Pediatra", "Clínica Médica"),
			new("Ginecólogo", "Clínica Médica"),
			new("Traumatólogo", "Cirugía y Ortopedia"),
			new("Neurólogo", "Clínica Médica"),
			new("Dermatólogo", "Clínica Médica"),
			new("Oftalmólogo", "Clínica Médica"),
			new("Otorrinolaringólogo", "Clínica Médica"),
			new("Psiquiatra", "Salud Mental"),
			new("Psicólogo", "Salud Mental"),
			new("Cirujano", "Cirugía General"),
			new("Kinesiólogo", "Rehabilitación"),
			new("Nutricionista", "Salud General")
		}.ToImmutableHashSet();

	public static Result<MedicoEspecialidad2025> Crear(string titulo, string rama) {
		if (string.IsNullOrWhiteSpace(titulo))
			return new Result<MedicoEspecialidad2025>.Error("El título no puede estar vacío.");
		if (string.IsNullOrWhiteSpace(rama))
			return new Result<MedicoEspecialidad2025>.Error("La rama no puede estar vacía.");

		var candidato = new MedicoEspecialidad2025(titulo.Trim(), rama.Trim());
		if (!EspecialidadesValidas.Contains(candidato))
			return new Result<MedicoEspecialidad2025>.Error("La especialidad médica no es válida.");

		return new Result<MedicoEspecialidad2025>.Ok(candidato);
	}
}
