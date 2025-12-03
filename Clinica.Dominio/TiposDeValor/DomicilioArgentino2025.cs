using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;


public readonly record struct DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad,
	string DireccionValor
) : IComoTexto {
	public string ATexto() => $"{DireccionValor}, {Localidad.ATexto()}";
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
	string NombreValor,
	ProvinciaArgentina2025 Provincia
) : IComoTexto {
	public string ATexto() => $"{NombreValor}, {Provincia.ATexto()}";
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




public enum ProvinciaCodigo2025 : byte {
	BuenosAires = 1,
	CiudadAutonomaBuenosAires = 2,
	Catamarca = 3,
	Chaco = 4,
	Chubut = 5,
	Cordoba = 6,
	Corrientes = 7,
	EntreRios = 8,
	Formosa = 9,
	Jujuy = 10,
	LaPampa = 11,
	LaRioja = 12,
	Mendoza = 13,
	Misiones = 14,
	Neuquen = 15,
	RioNegro = 16,
	Salta = 17,
	SanJuan = 18,
	SanLuis = 19,
	SantaCruz = 20,
	SantaFe = 21,
	SantiagoDelEstero = 22,
	TierraDelFuego = 23,
	Tucuman = 24
}

public readonly record struct ProvinciaArgentina2025(ProvinciaCodigo2025 CodigoInternoValor, string NombreValor) : IComoTexto {
	public string ATexto() => NombreValor;

	// Diccionario privado para lookup rápido
	private static readonly IReadOnlyDictionary<ProvinciaCodigo2025, string> _nombresPorCodigo = new Dictionary<ProvinciaCodigo2025, string>
	{
		{ ProvinciaCodigo2025.BuenosAires, "Buenos Aires" },
		{ ProvinciaCodigo2025.CiudadAutonomaBuenosAires, "Ciudad Autónoma de Buenos Aires" },
		{ ProvinciaCodigo2025.Catamarca, "Catamarca" },
		{ ProvinciaCodigo2025.Chaco, "Chaco" },
		{ ProvinciaCodigo2025.Chubut, "Chubut" },
		{ ProvinciaCodigo2025.Cordoba, "Córdoba" },
		{ ProvinciaCodigo2025.Corrientes, "Corrientes" },
		{ ProvinciaCodigo2025.EntreRios, "Entre Ríos" },
		{ ProvinciaCodigo2025.Formosa, "Formosa" },
		{ ProvinciaCodigo2025.Jujuy, "Jujuy" },
		{ ProvinciaCodigo2025.LaPampa, "La Pampa" },
		{ ProvinciaCodigo2025.LaRioja, "La Rioja" },
		{ ProvinciaCodigo2025.Mendoza, "Mendoza" },
		{ ProvinciaCodigo2025.Misiones, "Misiones" },
		{ ProvinciaCodigo2025.Neuquen, "Neuquén" },
		{ ProvinciaCodigo2025.RioNegro, "Río Negro" },
		{ ProvinciaCodigo2025.Salta, "Salta" },
		{ ProvinciaCodigo2025.SanJuan, "San Juan" },
		{ ProvinciaCodigo2025.SanLuis, "San Luis" },
		{ ProvinciaCodigo2025.SantaCruz, "Santa Cruz" },
		{ ProvinciaCodigo2025.SantaFe, "Santa Fe" },
		{ ProvinciaCodigo2025.SantiagoDelEstero, "Santiago del Estero" },
		{ ProvinciaCodigo2025.TierraDelFuego, "Tierra del Fuego" },
		{ ProvinciaCodigo2025.Tucuman, "Tucumán" }
	};

	private static readonly IReadOnlyDictionary<string, ProvinciaCodigo2025> _codigoPorNombre =
		_nombresPorCodigo.ToDictionary(kvp => kvp.Value.ToLowerInvariant(), kvp => kvp.Key);

	//public static IReadOnlyCollection<string> ProvinciasValidas => _nombresPorCodigo.Values;

	// ---------------- FACTORY POR CÓDIGO ----------------
	public static Result<ProvinciaArgentina2025> CrearPorCodigo(ProvinciaCodigo2025 codigo) {
		if (_nombresPorCodigo.TryGetValue(codigo, out var nombre))
			return new Result<ProvinciaArgentina2025>.Ok(new ProvinciaArgentina2025(codigo, nombre));

		return new Result<ProvinciaArgentina2025>.Error($"Código de provincia inválido: {codigo}");
	}

	public static Result<ProvinciaArgentina2025> CrearPorCodigo(byte? codigo) {
		if (codigo is not byte value)
			return new Result<ProvinciaArgentina2025>.Error("Código de provincia missing.");

		if (!Enum.IsDefined(typeof(ProvinciaCodigo2025), value))
			return new Result<ProvinciaArgentina2025>.Error($"Código de provincia inválido: {value}");

		return CrearPorCodigo((ProvinciaCodigo2025)value);
	}

	// ---------------- FACTORY POR NOMBRE ----------------
	public static Result<ProvinciaArgentina2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaArgentina2025>.Error("La provincia no puede estar vacía.");

		var normalizado = input.Trim().ToLowerInvariant();
		if (_codigoPorNombre.TryGetValue(normalizado, out ProvinciaCodigo2025 codigo))
			return CrearPorCodigo(codigo);

		return new Result<ProvinciaArgentina2025>.Error($"Provincia inválida: '{input}'.");
	}

	public static bool EsValida(string input) => _codigoPorNombre.ContainsKey(input.Trim().ToLowerInvariant());
}
