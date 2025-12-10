using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio._Disabled;

public record Nombre2025(
	string Valor
) {
    private static readonly int MaxLongitud = 50; // razonable, pero configurable
	public static Result<Nombre2025> CrearResult(string? nombre) {
		if (string.IsNullOrWhiteSpace(nombre))
			return new Result<Nombre2025>.Error("El nombre no puede estar vacío.");

		string nombreNorm = Normalize(nombre);

		if (nombreNorm.Length > MaxLongitud)
			return new Result<Nombre2025>.Error($"El nombre es demasiado largo (máximo {MaxLongitud} caracteres).");

		return new Result<Nombre2025>.Ok(new(nombreNorm));
	}

	public static string Normalize(string input) => input.Trim(); //evneutalmente podria capitalizar palabras
}
