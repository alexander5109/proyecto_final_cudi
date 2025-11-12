using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public readonly record struct LocalidadDeProvinciaType(
	string Nombre, 
	ProvinciaDeArgentinaType Provincia
);
public static class LocalidadDeProvincia2025{
	public static Result<LocalidadDeProvinciaType> Crear(string nombreLocalidad,Result<ProvinciaDeArgentinaType> provinciaResult) {
		if (string.IsNullOrWhiteSpace(nombreLocalidad))
			return new Result<LocalidadDeProvinciaType>.Error("El nombre de la localidad no puede estar vacío.");

		if (provinciaResult is Result<ProvinciaDeArgentinaType>.Error err)
			return new Result<LocalidadDeProvinciaType>.Error($"Provincia inválida: {err.Mensaje}");

		var provincia = ((Result<ProvinciaDeArgentinaType>.Ok)provinciaResult).Valor;

		return new Result<LocalidadDeProvinciaType>.Ok(new(nombreLocalidad, provincia));
	}
	public static string Normalize(string nombreLocalidad) => char.ToUpper(nombreLocalidad[0]) + nombreLocalidad[1..].ToLower();
}
