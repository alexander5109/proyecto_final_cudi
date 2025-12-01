using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.Comun.ValidatedExtensions;

namespace Clinica.Dominio.Entidades;


public record struct PacienteId(int Valor) {
	public static Result<PacienteId> Crear(int? id) =>
		id is int idGood
		? new Result<PacienteId>.Ok(new PacienteId(idGood))
		: new Result<PacienteId>.Error("El id no puede ser nulo.");
	public static bool TryParse(string? s, out PacienteId id) {
		if (int.TryParse(s, out int value)) {
			id = new PacienteId(value);
			return true;
		}
		id = default;
		return false;
	}
	public override string ToString() {
		return Valor.ToString();
	}

}

public record Paciente2025(
	PacienteId Id,
	NombreCompleto2025 NombreCompleto,
	DniArgentino2025 Dni,
	Contacto2025 Contacto,
	DomicilioArgentino2025 Domicilio,
	FechaDeNacimiento2025 FechaNacimiento,
	FechaRegistro2025 FechaIngreso
) {
	public static Result<Paciente2025> Crear(
		Result<PacienteId> idResult,
		Result<NombreCompleto2025> nombreResult,
		Result<DniArgentino2025> dniResult,
		Result<Contacto2025> contactoResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<FechaDeNacimiento2025> fechaNacimientoResult,
		Result<FechaRegistro2025> fechaIngresoResult
	) {
		Validated<(PacienteId, NombreCompleto2025, DniArgentino2025, Contacto2025, DomicilioArgentino2025, FechaDeNacimiento2025, FechaRegistro2025)> validado =
			ValidatedCombine.Combine(
				idResult.ToValidated(),
				nombreResult.ToValidated(),
				dniResult.ToValidated(),
				contactoResult.ToValidated(),
				domicilioResult.ToValidated(),
				fechaNacimientoResult.ToValidated(),
				fechaIngresoResult.ToValidated()
			);

		return validado switch {
			Validated<(PacienteId,
					   NombreCompleto2025,
					   DniArgentino2025,
					   Contacto2025,
					   DomicilioArgentino2025,
					   FechaDeNacimiento2025,
					   FechaRegistro2025)>.Valid v
				=> new Result<Paciente2025>.Ok(
						new Paciente2025(
							v.Value.Item1,
							v.Value.Item2,
							v.Value.Item3,
							v.Value.Item4,
							v.Value.Item5,
							v.Value.Item6,
							v.Value.Item7
						)
					),

			Validated<(PacienteId,
					   NombreCompleto2025,
					   DniArgentino2025,
					   Contacto2025,
					   DomicilioArgentino2025,
					   FechaDeNacimiento2025,
					   FechaRegistro2025)>.Invalid e
				=> new Result<Paciente2025>.Error(
						string.Join("\n", e.Errors)
					),
			_ => throw new InvalidOperationException()
		};
	}



	//public static Result<Paciente2025> Crear(
	//	Result<PacienteId> idResult,
	//	Result<NombreCompleto2025> nombreResult,
	//	Result<DniArgentino2025> dniResult,
	//	Result<Contacto2025> contactoResult,
	//	Result<DomicilioArgentino2025> domicilioResult,
	//	Result<FechaDeNacimiento2025> fechaNacimientoResult,
	//	Result<FechaRegistro2025> fechaIngresoResult
	//)
	//=>
	//	from id in idResult
	//	from nombre in nombreResult
	//	from dni in dniResult
	//	from contacto in contactoResult
	//	from domicilio in domicilioResult
	//	from fechaNac in fechaNacimientoResult
	//	from fechaIng in fechaIngresoResult
	//	select new Paciente2025(
	//		id,
	//		nombre,
	//		dni,
	//		contacto,
	//		domicilio,
	//		fechaNac,
	//		fechaIng
	//	);
}
