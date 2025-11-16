using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct MedicoEspecialidadRama2025(
	string Titulo
){
	// Conjunto inmutable de especialidades válidas
	public static readonly IImmutableSet<MedicoEspecialidadRama2025> RamasValidas = new HashSet<MedicoEspecialidadRama2025>{
		new("Clínica Médica"),
		new("Cirugía y Ortopedia"),
		new("Salud Mental"),
		new("Cirugía General"),
		new("Rehabilitación"),
		new("Salud General"),
		new ("Cirugía General"),
	}.ToImmutableHashSet();

	public static Result<MedicoEspecialidadRama2025> Crear(string rama) {
		if (string.IsNullOrWhiteSpace(rama))
			return new Result<MedicoEspecialidadRama2025>.Error("No se indicó el nombre de la rama medica.");

		var candidato = new MedicoEspecialidadRama2025(rama.Trim());
		if (!RamasValidas.Contains(candidato))
			return new Result<MedicoEspecialidadRama2025>.Error("La especialidad médica no es válida.");

		return new Result<MedicoEspecialidadRama2025>.Ok(candidato);
	}
}
