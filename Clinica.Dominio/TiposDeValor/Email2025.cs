using System.Text.RegularExpressions;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct Email2025(
	string Valor
) : IComoTexto {
	public string ATexto() {
		return Valor;
	}
	public static Result<Email2025> CrearResult(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<Email2025>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<Email2025>.Error("Correo electrónico inválido.");

		return new Result<Email2025>.Ok(new(input.Trim()));
	}
}
