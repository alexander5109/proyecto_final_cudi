using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio._Disabled;

public record struct Apellido2025(
	string Valor
) {
    private static readonly int MaxLongitud = 50; // razonable, pero configurable
	public static Result<Apellido2025> CrearResult(string? apellido) {

		if (string.IsNullOrWhiteSpace(apellido))
			return new Result<Apellido2025>.Error("El apellido no puede estar vacío.");

		string apellidoNorm = Normalize(apellido);

		if (apellidoNorm.Length > MaxLongitud)
			return new Result<Apellido2025>.Error($"El apellido es demasiado largo (máximo {MaxLongitud} caracteres).");

		return new Result<Apellido2025>.Ok(new(apellidoNorm));
	}

	public static string Normalize(string input) => input.Trim(); //evneutalmente podria capitalizar palabras
}
