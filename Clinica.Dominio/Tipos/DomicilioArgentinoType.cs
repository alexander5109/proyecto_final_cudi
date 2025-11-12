using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public readonly record struct DomicilioArgentinoType(
	LocalidadDeProvinciaType Localidad, 
	string Direccion
);
public static class DomicilioArgentino2025 {
	public static Result<DomicilioArgentinoType> Crear(Result<LocalidadDeProvinciaType> localidadResult, string? direccionTexto) {
		if (localidadResult is Result<LocalidadDeProvinciaType>.Error localidadError)
			return new Result<DomicilioArgentinoType>.Error(localidadError.Mensaje);

		var localidad = ((Result<LocalidadDeProvinciaType>.Ok)localidadResult).Valor;

		return new Result<DomicilioArgentinoType>.Ok(
			new DomicilioArgentinoType(
				localidad,
				Normalize(direccionTexto)
			)
		);
	}
	public static string Normalize(string value) => value.Trim();
}
