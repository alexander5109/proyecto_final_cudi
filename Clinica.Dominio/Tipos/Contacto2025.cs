using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

public record struct Contacto2025(
	Contacto2025CorreoElectronico Email,
	Contacto2025Telefono Telefono
) : IValidate<Contacto2025> {
	public static Result<Contacto2025> Crear(Result<Contacto2025CorreoElectronico> emailResult, Result<Contacto2025Telefono> telResult) 
		=> emailResult.Bind(emailOk => telResult.Map(telOk => new Contacto2025(emailOk, telOk)));

	//public Result<Contacto2025> Validate() => this.Email.Validate().Bind(emailOk => this.Telefono.Validate().Map(telOk => this));
}
