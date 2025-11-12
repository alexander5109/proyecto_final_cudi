using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public readonly record struct NombreCompletoType(
	string Nombre,
	string Apellido
);

public static class NombreCompleto2025 {
	static readonly int MaxLongitud = 100; // razonable, pero configurable

	public static Result<NombreCompletoType> Crear(string? nombre, string? apellido) {
		if (string.IsNullOrWhiteSpace(nombre))
			return new Result<NombreCompletoType>.Error("El nombre no puede estar vacío.");

		if (string.IsNullOrWhiteSpace(apellido))
			return new Result<NombreCompletoType>.Error("El apellido no puede estar vacío.");

		string nombreNorm = Normalize(nombre);
		string apellidoNorm = Normalize(apellido);

		if (nombreNorm.Length > MaxLongitud)
			return new Result<NombreCompletoType>.Error($"El nombre es demasiado largo (máximo {MaxLongitud} caracteres).");

		if (apellidoNorm.Length > MaxLongitud)
			return new Result<NombreCompletoType>.Error($"El apellido es demasiado largo (máximo {MaxLongitud} caracteres).");

		return new Result<NombreCompletoType>.Ok(new(nombreNorm, apellidoNorm));
	}

	public static string Normalize(string input) => input.Trim(); //evneutalmente podria capitalizar palabras
}
