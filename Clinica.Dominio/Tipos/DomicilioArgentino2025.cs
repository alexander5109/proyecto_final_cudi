using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public readonly record struct DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad, 
	string Direccion
) {
	public static Result<DomicilioArgentino2025> Crear(string provinciaTexto, string localidadTexto, string direccionTexto) {
		if (string.IsNullOrWhiteSpace(direccionTexto))
			return new Result<DomicilioArgentino2025>.Error("La dirección no puede estar vacía.");
		if (direccionTexto.Length > 100)
			return new Result<DomicilioArgentino2025>.Error("La dirección es demasiado larga.");

		Result<ProvinciaDeArgentina2025> provinciaResult = ProvinciaDeArgentina2025.Crear(provinciaTexto);
		Result<LocalidadDeProvincia2025> localidadResult = LocalidadDeProvincia2025.Crear(localidadTexto, provinciaResult);

		if (localidadResult is Result<LocalidadDeProvincia2025>.Error localidadError)
			return new Result<DomicilioArgentino2025>.Error(localidadError.Mensaje);

		var localidad = ((Result<LocalidadDeProvincia2025>.Ok)localidadResult).Value;

		return new Result<DomicilioArgentino2025>.Ok(
			new DomicilioArgentino2025(
				localidad,
				direccionTexto.Trim()
			)
		);
	}

	public override string ToString() => $"{Direccion}, {Localidad}";
}
