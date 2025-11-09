using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct NombreCompleto2025 {
	private readonly string _nombre;
	private readonly string _apellido;

	public string Nombre => _nombre;
	public string Apellido => _apellido;

	private NombreCompleto2025(string nombre, string apellido) {
		_nombre = nombre;
		_apellido = apellido;
	}

	public static Result<NombreCompleto2025> Crear(string nombre, string apellido) {
		if (string.IsNullOrWhiteSpace(nombre))
			return new Result<NombreCompleto2025>.Error("El nombre no puede estar vacío.");
		if (string.IsNullOrWhiteSpace(apellido))
			return new Result<NombreCompleto2025>.Error("El apellido no puede estar vacío.");
		if (nombre.Length > 50 || apellido.Length > 50)
			return new Result<NombreCompleto2025>.Error("El nombre o apellido es demasiado largo.");

		return new Result<NombreCompleto2025>.Ok(new(nombre.Trim(), apellido.Trim()));
	}

	public override string ToString() => $"{_nombre} {_apellido}";
}
