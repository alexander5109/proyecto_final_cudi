using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;

namespace Clinica.Dominio.TiposDeValor;


public readonly record struct DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad,
	string DireccionValor
) : IComoTexto {
	public string ATexto() => $"{DireccionValor}, {Localidad.ATexto()}";

	public static Result<DomicilioArgentino2025> CrearResult(
		Result<LocalidadDeProvincia2025> localidadResult,
		string? direccionTexto
	) =>
		direccionTexto.ToResult(!string.IsNullOrWhiteSpace(direccionTexto), "La dirección no puede estar vacía")
		.Bind(_ => localidadResult)
		.Map(localidad =>
			new DomicilioArgentino2025(
				localidad,
				Normalize(direccionTexto!)
			)
		);
	public static string Normalize(string value) => value.Trim();
}

