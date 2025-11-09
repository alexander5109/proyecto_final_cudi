using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Types;

public readonly record struct FechaDeNacimiento {
	private readonly DateTime _value;
	private FechaDeNacimiento(DateTime value) => _value = value;

	public static Result<FechaDeNacimiento> Crear(DateTime fecha) {
		if (fecha > DateTime.Now)
			return new Result<FechaDeNacimiento>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < DateTime.Now.AddYears(-120))
			return new Result<FechaDeNacimiento>.Error("Edad no válida.");

		return new Result<FechaDeNacimiento>.Ok(new(fecha));
	}

	public int Edad => (int)((DateTime.Now - _value).TotalDays / 365.25);

	public override string ToString() => _value.ToShortDateString();
}


