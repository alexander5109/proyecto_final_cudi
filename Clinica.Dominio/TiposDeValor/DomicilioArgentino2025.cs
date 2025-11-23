using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;


public readonly record struct DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad,
	string Direccion
): IComoTexto {
	public string ATexto() => $"{Direccion}, {Localidad.ATexto()}";
	public static Result<DomicilioArgentino2025> Crear(Result<LocalidadDeProvincia2025> localidadResult, string? direccionTexto) {
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


public readonly record struct LocalidadDeProvincia2025(
	string Nombre,
	ProvinciaArgentina2025 Provincia
) : IComoTexto {
	public string ATexto() => $"{Nombre}, {Provincia.ATexto()}";
	public static Result<LocalidadDeProvincia2025> Crear(string? nombreLocalidad, Result<ProvinciaArgentina2025> provinciaResult) {
		if (string.IsNullOrWhiteSpace(nombreLocalidad))
			return new Result<LocalidadDeProvincia2025>.Error("El nombre de la localidad no puede estar vacío.");

		if (provinciaResult is Result<ProvinciaArgentina2025>.Error err)
			return new Result<LocalidadDeProvincia2025>.Error($"Provincia inválida: {err.Mensaje}");

        ProvinciaArgentina2025 provincia = ((Result<ProvinciaArgentina2025>.Ok)provinciaResult).Valor;

		return new Result<LocalidadDeProvincia2025>.Ok(new(nombreLocalidad, provincia));
	}
	public static string Normalize(string nombreLocalidad) => char.ToUpper(nombreLocalidad[0]) + nombreLocalidad[1..].ToLower();
}


public readonly record struct ProvinciaArgentina2025(
	byte CodigoInterno,
	string Nombre
) : IComoTexto {
	public string ATexto() => Nombre;

	// --- Diccionario canónico ---
	private static readonly Dictionary<byte, string> _provinciasPorCodigo = new()
	{
		{ 1, "Buenos Aires" },
		{ 2, "Ciudad Autónoma de Buenos Aires" },
		{ 3, "Catamarca" },
		{ 4, "Chaco" },
		{ 5, "Chubut" },
		{ 6, "Córdoba" },
		{ 7, "Corrientes" },
		{ 8, "Entre Ríos" },
		{ 9, "Formosa" },
		{ 10, "Jujuy" },
		{ 11, "La Pampa" },
		{ 12, "La Rioja" },
		{ 13, "Mendoza" },
		{ 14, "Misiones" },
		{ 15, "Neuquén" },
		{ 16, "Río Negro" },
		{ 17, "Salta" },
		{ 18, "San Juan" },
		{ 19, "San Luis" },
		{ 20, "Santa Cruz" },
		{ 21, "Santa Fe" },
		{ 22, "Santiago del Estero" },
		{ 23, "Tierra del Fuego" },
		{ 24, "Tucumán" }
	};

	private static readonly Dictionary<string, byte> _codigoPorNombre =
		_provinciasPorCodigo
			.ToDictionary(kvp => kvp.Value.ToLowerInvariant(), kvp => kvp.Key);

	public static IReadOnlyCollection<string> ProvinciasValidas =>
		_provinciasPorCodigo.Values;
	public static Result<ProvinciaArgentina2025> CrearPorCodigo(byte? codigo) {
		if (codigo is not byte key) {
			return new Result<ProvinciaArgentina2025>.Error(
				$"Código de provincia missing: {codigo}."
			);
		}

		if (_provinciasPorCodigo.TryGetValue(key, out string? nombre)) {
			return new Result<ProvinciaArgentina2025>.Ok(
				new ProvinciaArgentina2025(key, nombre)
			);
		}

		return new Result<ProvinciaArgentina2025>.Error(
			$"Código de provincia inválido: {codigo}."
		);
	}

	// ---------------- FACTORY POR NOMBRE ----------------
	public static Result<ProvinciaArgentina2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaArgentina2025>.Error(
				"La provincia no puede estar vacía."
			);

		string normalizado = input.Trim().ToLowerInvariant();

		if (_codigoPorNombre.TryGetValue(normalizado, out byte codigo))
			return CrearPorCodigo(codigo);

		return new Result<ProvinciaArgentina2025>.Error(
			$"Provincia inválida: '{input}'."
		);
	}
	public static bool EsValida(string input)
		=> _codigoPorNombre.ContainsKey(input.Trim().ToLowerInvariant());
}
