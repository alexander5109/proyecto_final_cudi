using Clinica.Dominio.Comun;
namespace Clinica.Dominio.TiposDeValor;

public readonly record struct DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad, 
	string Direccion
){
	public static Result<DomicilioArgentino2025> Crear(Result<LocalidadDeProvincia2025> localidadResult, string? direccionTexto) {
		if (string.IsNullOrWhiteSpace(direccionTexto))
				return new Result<DomicilioArgentino2025>.Error("La dirección no puede estar vacía");


		if (localidadResult is Result<LocalidadDeProvincia2025>.Error localidadError)
			return new Result<DomicilioArgentino2025>.Error(localidadError.Mensaje);

		var localidad = ((Result<LocalidadDeProvincia2025>.Ok)localidadResult).Valor;

		return new Result<DomicilioArgentino2025>.Ok(
			new DomicilioArgentino2025(
				localidad,
				Normalize(direccionTexto)
			)
		);
	}
	public static string Normalize(string value) => value.Trim();
}
