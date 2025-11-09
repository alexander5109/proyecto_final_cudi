using Clinica.Dominio.Comun;
using Clinica.Dominio.Types;

public record struct Contacto {
	public CorreoElectronico Email { get; }
	public NumeroDeTelefono Telefono { get; }

	private Contacto(CorreoElectronico email, NumeroDeTelefono telefono) {
		Email = email;
		Telefono = telefono;
	}

	public static Result<Contacto> Crear(string email, string telefono) {
		var emailResult = CorreoElectronico.Crear(email);
		var telResult = NumeroDeTelefono.Crear(telefono);

		if (emailResult is Result<CorreoElectronico>.Error e1)
			return new Result<Contacto>.Error(e1.Message);

		if (telResult is Result<NumeroDeTelefono>.Error e2)
			return new Result<Contacto>.Error(e2.Message);

		var emailOk = ((Result<CorreoElectronico>.Ok)emailResult).Value;
		var telOk = ((Result<NumeroDeTelefono>.Ok)telResult).Value;

		return new Result<Contacto>.Ok(new Contacto(emailOk, telOk));
	}
}
