using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;

namespace Clinica.Dominio.TiposDeEnum;

public enum ProvinciaCodigo : byte {
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



public record struct ProvinciaArgentina2025(ProvinciaCodigo CodigoInternoValor, string NombreValor) : IComoTexto {
	public string ATexto() => NombreValor;
	public static IReadOnlyList<ProvinciaArgentina2025> Todas() => [.. _nombresPorCodigo.Select(kvp => new ProvinciaArgentina2025(kvp.Key, kvp.Value))];

	// Diccionario privado para lookup rápido
	private static readonly IReadOnlyDictionary<ProvinciaCodigo, string> _nombresPorCodigo = new Dictionary<ProvinciaCodigo, string>
	{
		{ ProvinciaCodigo.BuenosAires, "Buenos Aires" },
		{ ProvinciaCodigo.CiudadAutonomaBuenosAires, "Ciudad Autónoma de Buenos Aires" },
		{ ProvinciaCodigo.Catamarca, "Catamarca" },
		{ ProvinciaCodigo.Chaco, "Chaco" },
		{ ProvinciaCodigo.Chubut, "Chubut" },
		{ ProvinciaCodigo.Cordoba, "Córdoba" },
		{ ProvinciaCodigo.Corrientes, "Corrientes" },
		{ ProvinciaCodigo.EntreRios, "Entre Ríos" },
		{ ProvinciaCodigo.Formosa, "Formosa" },
		{ ProvinciaCodigo.Jujuy, "Jujuy" },
		{ ProvinciaCodigo.LaPampa, "La Pampa" },
		{ ProvinciaCodigo.LaRioja, "La Rioja" },
		{ ProvinciaCodigo.Mendoza, "Mendoza" },
		{ ProvinciaCodigo.Misiones, "Misiones" },
		{ ProvinciaCodigo.Neuquen, "Neuquén" },
		{ ProvinciaCodigo.RioNegro, "Río Negro" },
		{ ProvinciaCodigo.Salta, "Salta" },
		{ ProvinciaCodigo.SanJuan, "San Juan" },
		{ ProvinciaCodigo.SanLuis, "San Luis" },
		{ ProvinciaCodigo.SantaCruz, "Santa Cruz" },
		{ ProvinciaCodigo.SantaFe, "Santa Fe" },
		{ ProvinciaCodigo.SantiagoDelEstero, "Santiago del Estero" },
		{ ProvinciaCodigo.TierraDelFuego, "Tierra del Fuego" },
		{ ProvinciaCodigo.Tucuman, "Tucumán" }
	};

	private static readonly IReadOnlyDictionary<string, ProvinciaCodigo> _codigoPorNombre =
		_nombresPorCodigo.ToDictionary(kvp => kvp.Value.ToLowerInvariant(), kvp => kvp.Key);

	//public static IReadOnlyCollection<string> ProvinciasValidas => _nombresPorCodigo.Values;

	// ---------------- FACTORY POR CÓDIGO ----------------
	public static Result<ProvinciaArgentina2025> CrearResultPorCodigo(ProvinciaCodigo? codigoNulled) {
		if (codigoNulled is not ProvinciaCodigo codigo) {
			return new Result<ProvinciaArgentina2025>.Error($"Código de provincia vacío");
		}
		if (_nombresPorCodigo.TryGetValue(codigo, out string? nombre))
			return new Result<ProvinciaArgentina2025>.Ok(new ProvinciaArgentina2025(codigo, nombre));

		return new Result<ProvinciaArgentina2025>.Error($"Código de provincia inválido: {codigo}");
	}

	public static Result<ProvinciaArgentina2025> CrearResultPorCodigo(byte? codigo) {
		if (codigo is not byte value)
			return new Result<ProvinciaArgentina2025>.Error("Código de provincia missing.");

		if (!Enum.IsDefined(typeof(ProvinciaCodigo), value))
			return new Result<ProvinciaArgentina2025>.Error($"Código de provincia inválido: {value}");

		return CrearResultPorCodigo((ProvinciaCodigo)value);
	}

	// ---------------- FACTORY POR NOMBRE ----------------
	public static Result<ProvinciaArgentina2025> CrearResult(string? input) {
		
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaArgentina2025>.Error("La provincia no puede estar vacía.");

		string normalizado = input.Trim().ToLowerInvariant();
		if (_codigoPorNombre.TryGetValue(normalizado, out ProvinciaCodigo codigo))
			return CrearResultPorCodigo(codigo);

		return new Result<ProvinciaArgentina2025>.Error($"Provincia inválida: '{input}'.");
	}

	public static bool EsValida(string input) => _codigoPorNombre.ContainsKey(input.Trim().ToLowerInvariant());
}
