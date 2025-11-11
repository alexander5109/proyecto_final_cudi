using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public record struct Contacto2025(
	Contacto2025CorreoElectronico Email,
	Contacto2025Telefono Telefono
) {
	public static Result<Contacto2025> Crear(Result<Contacto2025CorreoElectronico> emailResult, Result<Contacto2025Telefono> telResult) 
		=> emailResult.Bind(emailOk => telResult.Map(telOk => new Contacto2025(emailOk, telOk)));
}
