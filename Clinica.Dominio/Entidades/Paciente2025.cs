using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public record struct Paciente2025EnDb(string Id, Paciente2025 Paciente);

public record struct Paciente2025(
	NombreCompleto2025 NombreCompleto,
	DniArgentino2025 Dni,
	Contacto2025 Contacto,
	DomicilioArgentino2025 Domicilio,
	FechaDeNacimiento2025 FechaNacimiento,
	FechaDeIngreso2025 FechaIngreso
) : IValidate<Paciente2025> {

	public static Result<Paciente2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<DniArgentino2025> dniResult,
		Result<Contacto2025> contactoResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<FechaDeNacimiento2025> fechaNacimientoResult,
		Result<FechaDeIngreso2025> fechaIngresoResult
	)
		=> nombreResult.Bind(nombreOk =>
		   dniResult.Bind(dniOk =>
		   contactoResult.Bind(contactoOk =>
		   domicilioResult.Bind(domicilioOk =>
		   fechaNacimientoResult.Bind(fechaNacOk =>
		   fechaIngresoResult.Map(fechaIngOk =>
			   new Paciente2025(
				   nombreOk,
				   dniOk,
				   contactoOk,
				   domicilioOk,
				   fechaNacOk,
				   fechaIngOk
			   )
		   ))))));

	//public Result<Paciente2025> Validate()
	//	=> NombreCompleto.Validate().Bind(nombreOk =>
	//	   Dni.Validate().Bind(dniOk =>
	//	   Contacto.Validate().Bind(contactoOk =>
	//	   Domicilio.Validate().Bind(domicilioOk =>
	//	   FechaNacimiento.Validate().Map(fechaOk =>
	//		   this
	//	   )))));
}
