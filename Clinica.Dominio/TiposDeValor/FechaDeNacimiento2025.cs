using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct FechaDeNacimiento2025(DateOnly Valor) {
	public static readonly DateTime Hoy = DateTime.Now;
	public static Result<FechaDeNacimiento2025> CrearResult(DateTime? fechaNulleable) {
		if (fechaNulleable is not DateTime fecha) {
			return new Result<FechaDeNacimiento2025>.Error("La fecha de ingreso no puede estar vacía.");
		}
		if (fecha > Hoy)
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < Hoy.AddYears(-120))
			return new Result<FechaDeNacimiento2025>.Error("Edad no válida (más de 120 años).");

		return new Result<FechaDeNacimiento2025>.Ok(new(DateOnly.FromDateTime(fecha)));
	}
}
