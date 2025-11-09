using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;
using Clinica.Dominio.Tipos;

public record struct Contacto2025 {
	public CorreoElectronico Email { get; }
	public NumeroDeTelefono2025 Telefono { get; }

	private Contacto2025(CorreoElectronico email, NumeroDeTelefono2025 telefono) {
		Email = email;
		Telefono = telefono;
	}

	public static Result<Contacto2025> Crear(string email, string telefono) {
		var emailResult = CorreoElectronico.Crear(email);
		var telResult = NumeroDeTelefono2025.Crear(telefono);

		if (emailResult is Result<CorreoElectronico>.Error e1)
			return new Result<Contacto2025>.Error(e1.Message);

		if (telResult is Result<NumeroDeTelefono2025>.Error e2)
			return new Result<Contacto2025>.Error(e2.Message);

		var emailOk = ((Result<CorreoElectronico>.Ok)emailResult).Value;
		var telOk = ((Result<NumeroDeTelefono2025>.Ok)telResult).Value;

		return new Result<Contacto2025>.Ok(new Contacto2025(emailOk, telOk));
	}
}
