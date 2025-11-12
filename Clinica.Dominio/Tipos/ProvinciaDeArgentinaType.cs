using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct ProvinciaDeArgentinaType(string Nombre);

public static class ProvinciaArgentina2025 {
	private static readonly HashSet<string> _provinciasValidas =
		new(StringComparer.OrdinalIgnoreCase)
		{
			"Buenos Aires",
			"Ciudad Autónoma de Buenos Aires",
			"Catamarca",
			"Chaco",
			"Chubut",
			"Córdoba",
			"Corrientes",
			"Entre Ríos",
			"Formosa",
			"Jujuy",
			"La Pampa",
			"La Rioja",
			"Mendoza",
			"Misiones",
			"Neuquén",
			"Río Negro",
			"Salta",
			"San Juan",
			"San Luis",
			"Santa Cruz",
			"Santa Fe",
			"Santiago del Estero",
			"Tierra del Fuego",
			"Tucumán"
		};

	public static Result<ProvinciaDeArgentinaType> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaDeArgentinaType>.Error("La provincia no puede estar vacía.");

		string normalizado = Normalizar(input);

		if (!_provinciasValidas.Contains(normalizado))
			return new Result<ProvinciaDeArgentinaType>.Error($"Provincia inválida: '{input}'.");

		return new Result<ProvinciaDeArgentinaType>.Ok(new ProvinciaDeArgentinaType(normalizado));
	}

	public static IReadOnlyCollection<string> ProvinciasValidas()
		=> _provinciasValidas.ToList().AsReadOnly();

	public static string Normalizar(string content)
		=> content.Trim();

	public static bool EsValida(string input)
		=> _provinciasValidas.Contains(Normalizar(input));

	public static string ToDisplayString(this ProvinciaDeArgentinaType provincia)
		=> provincia.Nombre;
}
