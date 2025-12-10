using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEnum;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct LocalidadDeProvincia2025(
	string NombreValor,
	ProvinciaArgentina2025 Provincia
) : IComoTexto {
	public string ATexto() => $"{NombreValor}, {Provincia.ATexto()}";
	public static Result<LocalidadDeProvincia2025> CrearResult(string? nombreLocalidad, Result<ProvinciaArgentina2025> provinciaResult) {
		if (string.IsNullOrWhiteSpace(nombreLocalidad))
			return new Result<LocalidadDeProvincia2025>.Error("El nombre de la localidad no puede estar vacío.");

		if (provinciaResult is Result<ProvinciaArgentina2025>.Error err)
			return new Result<LocalidadDeProvincia2025>.Error($"Provincia inválida: {err.Mensaje}");

		ProvinciaArgentina2025 provincia = ((Result<ProvinciaArgentina2025>.Ok)provinciaResult).Valor;

		return new Result<LocalidadDeProvincia2025>.Ok(new(nombreLocalidad, provincia));
	}
	public static string Normalize(string nombreLocalidad) => char.ToUpper(nombreLocalidad[0]) + nombreLocalidad[1..].ToLower();
}
