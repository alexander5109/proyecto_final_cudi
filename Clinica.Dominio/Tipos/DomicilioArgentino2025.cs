using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public record class DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad, 
	string Direccion
) : IValidate<DomicilioArgentino2025> {
	public static Result<DomicilioArgentino2025> Crear(Result<LocalidadDeProvincia2025> localidadResult, string? direccionTexto) {
		if (localidadResult is Result<LocalidadDeProvincia2025>.Error localidadError)
			return new Result<DomicilioArgentino2025>.Error(localidadError.Mensaje);

		var localidad = ((Result<LocalidadDeProvincia2025>.Ok)localidadResult).Value;

		return new Result<DomicilioArgentino2025>.Ok(
			new DomicilioArgentino2025(
				localidad,
				Normalize(direccionTexto)
			)
		);
	}
	public static string Normalize(string value) => value.Trim();

	public Result<DomicilioArgentino2025> Validate() {
		throw new NotImplementedException();


		return new Result<DomicilioArgentino2025>.Ok(this);
	}
}
