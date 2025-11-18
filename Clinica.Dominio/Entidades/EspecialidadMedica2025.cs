using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct EspecialidadCodigoInterno(int Valor);

public sealed class EspecialidadMedica2025 {
	// ============================
	//  Fields / Properties
	// ============================
	public EspecialidadCodigoInterno CodigoInterno { get; }
	public string Titulo { get; }
	public int DuracionConsultaMinutos { get; }

	public static List<EspecialidadMedica2025> TodasLasSoportadas => [.. PorId.Values];

	private EspecialidadMedica2025(
		EspecialidadCodigoInterno codigoInterno,
		string titulo,
		int duracionMin
	) {
		CodigoInterno = codigoInterno;
		Titulo = titulo;
		DuracionConsultaMinutos = duracionMin;
	}

	// ============================
	//  Static Data (Domain Truth)
	// ============================

	public static readonly string[] Titulos = [
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

	private static readonly Dictionary<string, int> DuracionesPorEspecialidad =
		new(StringComparer.OrdinalIgnoreCase) {
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


	// ============================================
	//  Bidirectional Dictionary (ID <-> Especialidad)
	// ============================================

	// Domain ID starts from 1…N, fixed permanently.
	private static readonly ImmutableDictionary<EspecialidadCodigoInterno, EspecialidadMedica2025> _fromId;
	private static readonly ImmutableDictionary<string, EspecialidadMedica2025> _fromTitulo;

	public static IReadOnlyDictionary<EspecialidadCodigoInterno, EspecialidadMedica2025> PorId => _fromId;
	public static IReadOnlyDictionary<string, EspecialidadMedica2025> PorTitulo => _fromTitulo;

	static EspecialidadMedica2025() {
		var byIdBuilder = ImmutableDictionary.CreateBuilder<EspecialidadCodigoInterno, EspecialidadMedica2025>();
		var byTituloBuilder = ImmutableDictionary.CreateBuilder<string, EspecialidadMedica2025>(StringComparer.OrdinalIgnoreCase);

		int idCounter = 1;
		foreach (var titulo in Titulos) {
			int dur = DuracionesPorEspecialidad.TryGetValue(titulo, out var x) ? x : 30;

			var esp = new EspecialidadMedica2025(
				new EspecialidadCodigoInterno(idCounter),
				titulo,
				dur
			);

			byIdBuilder.Add(esp.CodigoInterno, esp);
			byTituloBuilder.Add(esp.Titulo, esp);

			idCounter++;
		}

		_fromId = byIdBuilder.ToImmutable();
		_fromTitulo = byTituloBuilder.ToImmutable();
	}

	// ======================================================
	// Public API: Creation (Lookup only, no dynamic creation)
	// ======================================================

	public static Result<EspecialidadMedica2025> CrearPorTitulo(string? titulo) {
		if (string.IsNullOrWhiteSpace(titulo))
			return new Result<EspecialidadMedica2025>.Error("El título no puede estar vacío.");

		if (_fromTitulo.TryGetValue(titulo.Trim(), out var esp))
			return new Result<EspecialidadMedica2025>.Ok(esp);

		return new Result<EspecialidadMedica2025>.Error($"La especialidad '{titulo}' no está soportada por el dominio.");
	}

	public static Result<EspecialidadMedica2025> CrearPorCodigoInterno(int? id) {
		if (id is null)
			return new Result<EspecialidadMedica2025>.Error("El CodigoInterno no puede ser nulo.");
		var key = new EspecialidadCodigoInterno((int)id);

		if (_fromId.TryGetValue(key, out var esp))
			return new Result<EspecialidadMedica2025>.Ok(esp);

		return new Result<EspecialidadMedica2025>.Error($"No existe la especialidad con CodigoInterno = {id}.");
	}

	// ======================================================
	// Utility
	// ======================================================

	public override string ToString() => $"{Titulo} ({DuracionConsultaMinutos} min)";
}
