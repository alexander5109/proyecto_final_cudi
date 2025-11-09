using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct FechaDeNacimiento2025 {
	private readonly DateTime _value;
	private FechaDeNacimiento2025(DateTime value) => _value = value;

	public static Result<FechaDeNacimiento2025> Crear(DateTime fecha) {
		if (fecha > DateTime.Now)
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < DateTime.Now.AddYears(-120))
			return new Result<FechaDeNacimiento2025>.Error("Edad no válida.");

		return new Result<FechaDeNacimiento2025>.Ok(new(fecha));
	}

	public int Edad => (int)((DateTime.Now - _value).TotalDays / 365.25);

	public override string ToString() => _value.ToShortDateString();
}


