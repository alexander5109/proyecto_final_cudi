using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public record struct Contacto2025(
	ContactoEmail2025 Email,
	ContactoTelefono2025 Telefono
){
	public static Result<Contacto2025> Crear(Result<ContactoEmail2025> emailResult, Result<ContactoTelefono2025> telResult) 
		=> emailResult.Bind(emailOk => telResult.Map(telOk => new Contacto2025(emailOk, telOk)));
}
