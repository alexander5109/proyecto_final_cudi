using Clinica.Dominio.Comun;
using System.IO.IsolatedStorage;
namespace Clinica.Dominio.Tipos;

public record class LocalidadDeProvincia2025(
	string Nombre, 
	ProvinciaDeArgentina2025 Provincia
) : IValidate<LocalidadDeProvincia2025> {
	public static Result<LocalidadDeProvincia2025> Crear(string nombreLocalidad,Result<ProvinciaDeArgentina2025> provinciaResult) {
		if (string.IsNullOrWhiteSpace(nombreLocalidad))
			return new Result<LocalidadDeProvincia2025>.Error("El nombre de la localidad no puede estar vacío.");

		if (provinciaResult is Result<ProvinciaDeArgentina2025>.Error err)
			return new Result<LocalidadDeProvincia2025>.Error($"Provincia inválida: {err.Mensaje}");

		var provincia = ((Result<ProvinciaDeArgentina2025>.Ok)provinciaResult).Value;

		return new Result<LocalidadDeProvincia2025>.Ok(new(nombreLocalidad, provincia));
	}
	public static string Normalize(string nombreLocalidad) => char.ToUpper(nombreLocalidad[0]) + nombreLocalidad[1..].ToLower();
	public Result<LocalidadDeProvincia2025> Validate() {
		if (string.IsNullOrWhiteSpace(Nombre))
			return new Result<LocalidadDeProvincia2025>.Error("El nombre de la localidad no puede estar vacío.");

		if (Provincia.Validate() is Result<ProvinciaDeArgentina2025>.Error err)
			return new Result<LocalidadDeProvincia2025>.Error($"Provincia inválida: {err.Mensaje}");

		return new Result<LocalidadDeProvincia2025>.Ok(this);
	}
}
