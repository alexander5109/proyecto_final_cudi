using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public record struct ContactoType(
	ContactoEmailType Email,
	ContactoTelefonoType Telefono
);
public static class Contacto2025 {
	public static Result<ContactoType> Crear(Result<ContactoEmailType> emailResult, Result<ContactoTelefonoType> telResult) 
		=> emailResult.Bind(emailOk => telResult.Map(telOk => new ContactoType(emailOk, telOk)));
}
