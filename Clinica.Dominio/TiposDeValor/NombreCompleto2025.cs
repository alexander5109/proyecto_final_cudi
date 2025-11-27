using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct NombreCompleto2025(
	string NombreValor,
	string ApellidoValor
): IComoTexto {
	public string ATexto() => $"{ApellidoValor} {NombreValor}";

	static readonly int MaxLongitud = 100; // razonable, pero configurable
	public static Result<NombreCompleto2025> Crear(string? nombre, string? apellido) {
		if (string.IsNullOrWhiteSpace(nombre))
			return new Result<NombreCompleto2025>.Error("El nombre no puede estar vacío.");

		if (string.IsNullOrWhiteSpace(apellido))
			return new Result<NombreCompleto2025>.Error("El apellido no puede estar vacío.");

		string nombreNorm = Normalize(nombre);
		string apellidoNorm = Normalize(apellido);

		if (nombreNorm.Length > MaxLongitud)
			return new Result<NombreCompleto2025>.Error($"El nombre es demasiado largo (máximo {MaxLongitud} caracteres).");

		if (apellidoNorm.Length > MaxLongitud)
			return new Result<NombreCompleto2025>.Error($"El apellido es demasiado largo (máximo {MaxLongitud} caracteres).");

		return new Result<NombreCompleto2025>.Ok(new(nombreNorm, apellidoNorm));
	}

	public static string Normalize(string input) => input.Trim(); //evneutalmente podria capitalizar palabras
}
