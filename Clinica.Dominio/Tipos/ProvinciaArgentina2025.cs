using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct ProvinciaArgentina2025(
	string Nombre
){
	public static readonly HashSet<string> _provinciasValidas =
		new(StringComparer.OrdinalIgnoreCase){
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

	public static Result<ProvinciaArgentina2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaArgentina2025>.Error("La provincia no puede estar vacía.");

		string normalizado = Normalizar(input);

		if (!_provinciasValidas.Contains(normalizado))
			return new Result<ProvinciaArgentina2025>.Error($"Provincia inválida: '{input}'.");

		return new Result<ProvinciaArgentina2025>.Ok(new ProvinciaArgentina2025(normalizado));
	}

	public static IReadOnlyCollection<string> ProvinciasValidas()=> _provinciasValidas.ToList().AsReadOnly();
	public static string Normalizar(string content)
		=> content.Trim();
	public static bool EsValida(string input)=> _provinciasValidas.Contains(Normalizar(input));
}
