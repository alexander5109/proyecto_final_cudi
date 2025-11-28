using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;


public readonly record struct PacienteId(int Valor) {
	public static Result<PacienteId> Crear(int? id) => 
		id is int idGood
		? new Result<PacienteId>.Ok(new PacienteId(idGood))
		: new Result<PacienteId>.Error("El id no puede ser nulo.");
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
	)
	=>
		from id in idResult
		from nombre in nombreResult
		from dni in dniResult
		from contacto in contactoResult
		from domicilio in domicilioResult
		from fechaNac in fechaNacimientoResult
		from fechaIng in fechaIngresoResult
		select new Paciente2025(
			id,
			nombre,
			dni,
			contacto,
			domicilio,
			fechaNac,
			fechaIng
		);
}
