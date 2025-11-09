using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct ProvinciaDeArgentina2025(
	string Nombre
) {
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

	public static Result<ProvinciaDeArgentina2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaDeArgentina2025>.Error("La provincia no puede estar vacía.");

		string normalizado = input.Trim();

		if (!_provinciasValidas.Contains(normalizado))
			return new Result<ProvinciaDeArgentina2025>.Error($"Provincia inválida: '{input}'.");

		return new Result<ProvinciaDeArgentina2025>.Ok(new ProvinciaDeArgentina2025(normalizado));
	}

	public static IReadOnlyCollection<string> ListarPosibles()
		=> _provinciasValidas.ToList().AsReadOnly();

	public override string ToString() => Nombre;
}
