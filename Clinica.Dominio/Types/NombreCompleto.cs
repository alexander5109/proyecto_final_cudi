using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Types;

public readonly record struct NombreCompleto {
	private readonly string _nombre;
	private readonly string _apellido;

	public string Nombre => _nombre;
	public string Apellido => _apellido;

	private NombreCompleto(string nombre, string apellido) {
		_nombre = nombre;
		_apellido = apellido;
	}

	public static Result<NombreCompleto> Crear(string nombre, string apellido) {
		if (string.IsNullOrWhiteSpace(nombre))
			return new Result<NombreCompleto>.Error("El nombre no puede estar vacío.");
		if (string.IsNullOrWhiteSpace(apellido))
			return new Result<NombreCompleto>.Error("El apellido no puede estar vacío.");
		if (nombre.Length > 50 || apellido.Length > 50)
			return new Result<NombreCompleto>.Error("El nombre o apellido es demasiado largo.");

		return new Result<NombreCompleto>.Ok(new(nombre.Trim(), apellido.Trim()));
	}

	public override string ToString() => $"{_nombre} {_apellido}";
}
