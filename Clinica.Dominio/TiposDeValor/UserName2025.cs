using Clinica.Dominio.FunctionalToolkit;


namespace Clinica.Dominio.TiposDeValor;

public readonly record struct UserName2025(string Valor) {
	public static Result<UserName2025> CrearResult(string? raw) {
		if (string.IsNullOrWhiteSpace(raw))
			return new Result<UserName2025>.Error("El nombre de usuario no puede estar vacío.");

		if (raw.Length is < 5 or > 20)
			return new Result<UserName2025>.Error("El nombre de usuario debe tener entre 5 y 20 caracteres.");

		return new Result<UserName2025>.Ok(new UserName2025(raw));
	}
}
