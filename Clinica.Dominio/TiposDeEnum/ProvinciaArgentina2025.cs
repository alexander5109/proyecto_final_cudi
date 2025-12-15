using System.Collections.Frozen;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;

namespace Clinica.Dominio.TiposDeEnum;

public enum ProvinciaEnum : byte {
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



public record struct ProvinciaArgentina2025(ProvinciaEnum CodigoInternoValor, string NombreValor) : IComoTexto {
	public readonly string ATexto() => NombreValor;
	public static IReadOnlyList<ProvinciaArgentina2025> Todas() => [.. _nombresPorCodigo.Select(kvp => new ProvinciaArgentina2025(kvp.Key, kvp.Value))];

	// Diccionario privado para lookup rápido
	private static readonly FrozenDictionary<ProvinciaEnum, string> _nombresPorCodigo =
		new Dictionary<ProvinciaEnum, string> {
			[ProvinciaEnum.BuenosAires] = "Buenos Aires",
			[ProvinciaEnum.CiudadAutonomaBuenosAires] = "Ciudad Autónoma de Buenos Aires",
			[ProvinciaEnum.Catamarca] = "Catamarca",
			[ProvinciaEnum.Chaco] = "Chaco",
			[ProvinciaEnum.Chubut] = "Chubut",
			[ProvinciaEnum.Cordoba] = "Córdoba",
			[ProvinciaEnum.Corrientes] = "Corrientes",
			[ProvinciaEnum.EntreRios] = "Entre Ríos",
			[ProvinciaEnum.Formosa] = "Formosa",
			[ProvinciaEnum.Jujuy] = "Jujuy",
			[ProvinciaEnum.LaPampa] = "La Pampa",
			[ProvinciaEnum.LaRioja] = "La Rioja",
			[ProvinciaEnum.Mendoza] = "Mendoza",
			[ProvinciaEnum.Misiones] = "Misiones",
			[ProvinciaEnum.Neuquen] = "Neuquén",
			[ProvinciaEnum.RioNegro] = "Río Negro",
			[ProvinciaEnum.Salta] = "Salta",
			[ProvinciaEnum.SanJuan] = "San Juan",
			[ProvinciaEnum.SanLuis] = "San Luis",
			[ProvinciaEnum.SantaCruz] = "Santa Cruz",
			[ProvinciaEnum.SantaFe] = "Santa Fe",
			[ProvinciaEnum.SantiagoDelEstero] = "Santiago del Estero",
			[ProvinciaEnum.TierraDelFuego] = "Tierra del Fuego",
			[ProvinciaEnum.Tucuman] = "Tucumán"
		}.ToFrozenDictionary();

	private static readonly Dictionary<string, ProvinciaEnum> _codigoPorNombre = _nombresPorCodigo.ToDictionary(kvp => kvp.Value.ToLowerInvariant(), kvp => kvp.Key);

	//public static IReadOnlyCollection<string> ProvinciasValidas => _nombresPorCodigo.Values;

	// ---------------- FACTORY POR CÓDIGO ----------------
	public static Result<ProvinciaArgentina2025> CrearResultPorCodigo(ProvinciaEnum? codigoNulled) {
		if (codigoNulled is not ProvinciaEnum codigo) {
			return new Result<ProvinciaArgentina2025>.Error($"Código de provincia vacío");
		}
		if (_nombresPorCodigo.TryGetValue(codigo, out string? nombre))
			return new Result<ProvinciaArgentina2025>.Ok(new ProvinciaArgentina2025(codigo, nombre));

		return new Result<ProvinciaArgentina2025>.Error($"Código de provincia inválido: {codigo}");
	}

	public static Result<ProvinciaArgentina2025> CrearResultPorCodigo(byte? codigo) {
		if (codigo is not byte value)
			return new Result<ProvinciaArgentina2025>.Error("Código de provincia missing.");

		if (!Enum.IsDefined(typeof(ProvinciaEnum), value))
			return new Result<ProvinciaArgentina2025>.Error($"Código de provincia inválido: {value}");

		return CrearResultPorCodigo((ProvinciaEnum)value);
	}

	// ---------------- FACTORY POR NOMBRE ----------------
	public static Result<ProvinciaArgentina2025> CrearResult(string? input) {

		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaArgentina2025>.Error("La provincia no puede estar vacía.");

		string normalizado = input.Trim().ToLowerInvariant();
		if (_codigoPorNombre.TryGetValue(normalizado, out ProvinciaEnum codigo))
			return CrearResultPorCodigo(codigo);

		return new Result<ProvinciaArgentina2025>.Error($"Provincia inválida: '{input}'.");
	}

	public static bool EsValida(string input) => _codigoPorNombre.ContainsKey(input.Trim().ToLowerInvariant());
}
