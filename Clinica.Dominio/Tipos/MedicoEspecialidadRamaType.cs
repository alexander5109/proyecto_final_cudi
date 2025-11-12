using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoEspecialidadRamaType(
	string Titulo
);
public static class MedicoEspecialidadRama2025 {
	// Conjunto inmutable de especialidades válidas
	public static readonly IImmutableSet<MedicoEspecialidadRamaType> RamasValidas = new HashSet<MedicoEspecialidadRamaType>{
		new("Clínica Médica"),
		new("Cirugía y Ortopedia"),
		new("Salud Mental"),
		new("Cirugía General"),
		new("Rehabilitación"),
		new("Salud General"),
		new ("Cirugía General"),
	}.ToImmutableHashSet();

	public static Result<MedicoEspecialidadRamaType> Crear(string rama) {
		if (string.IsNullOrWhiteSpace(rama))
			return new Result<MedicoEspecialidadRamaType>.Error("No se indicó el nombre de la rama medica.");

		var candidato = new MedicoEspecialidadRamaType(rama.Trim());
		if (!RamasValidas.Contains(candidato))
			return new Result<MedicoEspecialidadRamaType>.Error("La especialidad médica no es válida.");

		return new Result<MedicoEspecialidadRamaType>.Ok(candidato);
	}
}
