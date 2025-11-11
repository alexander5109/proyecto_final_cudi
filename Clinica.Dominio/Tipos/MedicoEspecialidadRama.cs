using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoEspecialidadRama(
	string Titulo
) {
	// Conjunto inmutable de especialidades válidas
	public static readonly IImmutableSet<MedicoEspecialidadRama> RamasValidas =
		new HashSet<MedicoEspecialidadRama>
		{
			new("Clínica Médica"),
			new("Cirugía y Ortopedia"),
			new("Salud Mental"),
			new("Cirugía General"),
			new("Rehabilitación"),
			new("Salud General"),
			new ("Cirugía General"),
		}.ToImmutableHashSet();

	public static Result<MedicoEspecialidadRama> Crear(string rama) {
		if (string.IsNullOrWhiteSpace(rama))
			return new Result<MedicoEspecialidadRama>.Error("No se indicó el nombre de la rama medica.");

		var candidato = new MedicoEspecialidadRama(rama.Trim());
		if (!RamasValidas.Contains(candidato))
			return new Result<MedicoEspecialidadRama>.Error("La especialidad médica no es válida.");

		return new Result<MedicoEspecialidadRama>.Ok(candidato);
	}
}
