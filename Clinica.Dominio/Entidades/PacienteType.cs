using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;
namespace Clinica.Dominio.Entidades;

public readonly record struct PacienteType(
	NombreCompletoType NombreCompleto,
	DniArgentinoType Dni,
	ContactoType Contacto,
	DomicilioArgentinoType Domicilio,
	FechaDeNacimientoType FechaNacimiento,
	FechaIngresoType FechaIngreso
);

public static class Paciente2025 {
	public static Result<PacienteType> Crear(
		Result<NombreCompletoType> nombreResult,
		Result<DniArgentinoType> dniResult,
		Result<ContactoType> contactoResult,
		Result<DomicilioArgentinoType> domicilioResult,
		Result<FechaDeNacimientoType> fechaNacimientoResult,
		Result<FechaIngresoType> fechaIngresoResult
	)
	=> nombreResult.Bind(nombreOk =>
		dniResult.Bind(dniOk =>
		contactoResult.Bind(contactoOk =>
		domicilioResult.Bind(domicilioOk =>
		fechaNacimientoResult.Bind(fechaNacOk =>
		fechaIngresoResult.Map(fechaIngOk =>
			new PacienteType(
				nombreOk,
				dniOk,
				contactoOk,
				domicilioOk,
				fechaNacOk,
				fechaIngOk
			)
		))))));
}
