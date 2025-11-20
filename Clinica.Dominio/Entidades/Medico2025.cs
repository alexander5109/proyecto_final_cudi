using System.Globalization;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;

public record Medico2025(
		NombreCompleto2025 NombreCompleto,
		ListaEspecialidadesMedicas2025 Especialidades,
		DniArgentino2025 Dni,
		DomicilioArgentino2025 Domicilio,
		ContactoTelefono2025 Telefono,
		ListaHorarioMedicos2025 ListaHorarios,
		FechaIngreso2025 FechaIngreso,
		MedicoSueldoMinimo2025 SueldoMinimoGarantizado,
		bool HaceGuardias
	) {
		public static Result<Medico2025> Crear(
			Result<NombreCompleto2025> nombreResult,
			Result<ListaEspecialidadesMedicas2025> especialidadResult,
			Result<DniArgentino2025> dniResult,
			Result<DomicilioArgentino2025> domicilioResult,
			Result<ContactoTelefono2025> telefonoResult,
			Result<ListaHorarioMedicos2025> horariosResult,
			Result<FechaIngreso2025> fechaIngresoResult,
			Result<MedicoSueldoMinimo2025> sueldoResult,
			bool haceGuardia
		) =>
			from nombre in nombreResult
			from esp in especialidadResult
			from dni in dniResult
			from dom in domicilioResult
			from tel in telefonoResult
			from horarios in horariosResult
			from fechaIng in fechaIngresoResult
			from sueldo in sueldoResult
			select new Medico2025(
				nombre,
				esp,
				dni,
				dom,
				tel,
				horarios,
				fechaIng,
				sueldo,
				haceGuardia
			);
	}


public readonly record struct MedicoSueldoMinimo2025(
	double Valor
) : IComoTexto {
	public string ATexto() => Valor.ToString("C", CultureInfo.CurrentCulture);
	public const double MINIMO = 200_000;
	public const double MAXIMO = 5_000_000;
	public static Result<MedicoSueldoMinimo2025> Crear(double? input) {
		return input switch {
			null => new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede estar vacío."),
			< 0 => new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede ser negativo."),
			< MINIMO => new Result<MedicoSueldoMinimo2025>.Error($"El sueldo mínimo razonable es {MINIMO:N0}."),
			> MAXIMO => new Result<MedicoSueldoMinimo2025>.Error($"El sueldo ingresado ({input}) es excesivamente alto."),
			_ => new Result<MedicoSueldoMinimo2025>.Ok(new MedicoSueldoMinimo2025(input.Value))
		};
	}
	public static Result<MedicoSueldoMinimo2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede estar vacío.");

		var normalized = input.Trim();

		if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor) &&
			!double.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out valor)) {
			return new Result<MedicoSueldoMinimo2025>.Error($"Valor inválido: '{input}'. Debe ser un número.");
		}

		if (valor < 0)
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede ser negativo.");

		if (valor < MINIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo mínimo razonable es {MINIMO.ToString("N0")}.");

		if (valor > MAXIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo ingresado ({valor.ToString("N0")}) es excesivamente alto.");

		return new Result<MedicoSueldoMinimo2025>.Ok(new MedicoSueldoMinimo2025(valor));
	}

}
