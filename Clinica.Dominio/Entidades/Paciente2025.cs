using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Dominio.Comun.ValidatedExtensions;

namespace Clinica.Dominio.Entidades;


public readonly record struct PacienteId(int Valor) {
	public static Result<PacienteId> CrearResult(PacienteId? id) =>
		(id is PacienteId idGood && idGood.Valor >= 0)
		? new Result<PacienteId>.Ok(idGood)
		: new Result<PacienteId>.Error("El id no puede ser nulo o negativo.");
	public static Result<PacienteId> CrearResult(int? id) =>
		id is int idGood
		? new Result<PacienteId>.Ok(new PacienteId(idGood))
		: new Result<PacienteId>.Error("El id no puede ser nulo.");
	public static PacienteId Crear(int id) => new(id);
	public static bool TryParse(string? s, out PacienteId id) {
		if (int.TryParse(s, out int value)) {
			id = new PacienteId(value);
			return true;
		}
		id = default;
		return false;
	}
	public readonly override string ToString() {
		return Valor.ToString();
	}

}
public record Paciente2025Agg(
	PacienteId Id,
	Paciente2025 Paciente
) {
	public static Result<Paciente2025Agg> CrearResult(
		Result<PacienteId> idResult,
		Result<Paciente2025> pacienteResult
	)
		=> from id in idResult
		   from paciente in pacienteResult
		   select new Paciente2025Agg(
			   id,
			   paciente
		   );

}

public record Paciente2025(
	NombreCompleto2025 NombreCompleto,
	DniArgentino2025 Dni,
	Contacto2025 Contacto,
	DomicilioArgentino2025 Domicilio,
	FechaDeNacimiento2025 FechaNacimiento,
	DateTime FechaIngreso
) : IComoTexto {
	public static Result<Paciente2025> CrearResult(
		Result<NombreCompleto2025> nombreResult,
		Result<DniArgentino2025> dniResult,
		Result<Contacto2025> contactoResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<FechaDeNacimiento2025> fechaNacimientoResult,
		DateTime fechaIngreso
	) 
		=> nombreResult.BindWithPrefix(prefixError: "Error en NombreCompleto: \n", caseOk: nombre
		=> dniResult.BindWithPrefix(prefixError: "Error en Dni: \n", caseOk: dni
		=> contactoResult.BindWithPrefix(prefixError: "Error en Contacto: \n", caseOk: contacto
		=> domicilioResult.BindWithPrefix(prefixError: "Error en Domicilio: \n", caseOk: domicilio
		=> fechaNacimientoResult.BindWithPrefix(prefixError: "Error en FechaNacimiento: \n", caseOk: fechaNac
		=> new Result<Paciente2025>.Ok(new Paciente2025(
			nombre,
			dni,
			contacto,
			domicilio,
			fechaNac,
			fechaIngreso
		))
	)))));

	public string ATexto() {
		//Id: { Id.Valor}\n
		return @$"
			NombreCompleto: {NombreCompleto.ATexto()}\n
			Dni: {Dni.Valor}\n
			Contacto: {Contacto.ATexto()}\n
			Domicilio: {Domicilio.ATexto()}\n
			FechaNacimiento: {FechaNacimiento.ATexto()}\n
			FechaIngreso: {FechaIngreso.ATextoHoras()}\n
		";
	}


	//public static Result<Paciente2025> CrearResult(
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
