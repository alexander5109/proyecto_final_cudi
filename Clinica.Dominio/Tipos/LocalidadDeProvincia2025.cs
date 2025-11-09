using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public readonly record struct LocalidadDeProvincia2025(
	string Nombre, 
	ProvinciaDeArgentina2025 Provincia
) {
	public static Result<LocalidadDeProvincia2025> Crear(string nombreLocalidad,Result<ProvinciaDeArgentina2025> provinciaResult) {
		if (string.IsNullOrWhiteSpace(nombreLocalidad))
			return new Result<LocalidadDeProvincia2025>.Error("El nombre de la localidad no puede estar vacío.");

		if (provinciaResult is Result<ProvinciaDeArgentina2025>.Error err)
			return new Result<LocalidadDeProvincia2025>.Error($"Provincia inválida: {err.Mensaje}");

		var provincia = ((Result<ProvinciaDeArgentina2025>.Ok)provinciaResult).Value;
		string normalizado = char.ToUpper(nombreLocalidad[0]) + nombreLocalidad[1..].ToLower();

		return new Result<LocalidadDeProvincia2025>.Ok(new(normalizado, provincia));
	}
}
