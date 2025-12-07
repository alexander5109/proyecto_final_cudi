using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;


public readonly record struct DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad,
	string DireccionValor
) : IComoTexto {
	public string ATexto() => $"{DireccionValor}, {Localidad.ATexto()}";
	public static Result<DomicilioArgentino2025> CrearResult(Result<LocalidadDeProvincia2025> localidadResult, string? direccionTexto) {
		if (string.IsNullOrWhiteSpace(direccionTexto))
			return new Result<DomicilioArgentino2025>.Error("La dirección no puede estar vacía");


		if (localidadResult is Result<LocalidadDeProvincia2025>.Error localidadError)
			return new Result<DomicilioArgentino2025>.Error(localidadError.Mensaje);

		LocalidadDeProvincia2025 localidad = ((Result<LocalidadDeProvincia2025>.Ok)localidadResult).Valor;

		return new Result<DomicilioArgentino2025>.Ok(
			new DomicilioArgentino2025(
				localidad,
				Normalize(direccionTexto)
			)
		);
	}
	public static string Normalize(string value) => value.Trim();
}

