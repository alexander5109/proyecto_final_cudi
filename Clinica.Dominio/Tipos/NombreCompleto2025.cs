using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct NombreCompleto2025(
	string Nombre,
	string Apellido
) {
	static readonly int MaxLongitud = 100; // razonable, pero configurable

	public static Result<NombreCompleto2025> Crear(string? nombre, string? apellido) {
		if (string.IsNullOrWhiteSpace(nombre))
			return new Result<NombreCompleto2025>.Error("El nombre no puede estar vacío.");

		if (string.IsNullOrWhiteSpace(apellido))
			return new Result<NombreCompleto2025>.Error("El apellido no puede estar vacío.");

		nombre = nombre.Trim();
		apellido = apellido.Trim();

		if (nombre.Length > MaxLongitud)
			return new Result<NombreCompleto2025>.Error($"El nombre es demasiado largo (máximo {MaxLongitud} caracteres).");

		if (apellido.Length > MaxLongitud)
			return new Result<NombreCompleto2025>.Error($"El apellido es demasiado largo (máximo {MaxLongitud} caracteres).");

		// Podríamos también normalizar mayúsculas/minúsculas si quisieras.
		return new Result<NombreCompleto2025>.Ok(new(nombre, apellido));
	}
}
