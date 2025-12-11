using System.Text.RegularExpressions;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct Telefono2025(
	string Valor
) : IComoTexto {
	public string ATexto() {
		return Valor;
	}
	public static Result<Telefono2025> CrearResult(string? input){
		if (string.IsNullOrWhiteSpace(input))
			return new Result<Telefono2025>.Error("El teléfono no puede estar vacío.");

        string soloNumeros = Regex.Replace(input, @"\D", "");

		if (soloNumeros.Length > 10)
			return new Result<Telefono2025>.Error("El teléfono debe tener exactamente 10 dígitos (sin +54, sin espacios, sin guiones).");

		if (soloNumeros.Length < 10)
			return new Result<Telefono2025>.Error("El teléfono debe tener exactamente 10 dígitos (incluyendo el codigo de area, ej 11).");

		return new Result<Telefono2025>.Ok(new Telefono2025(soloNumeros));
	}
}
