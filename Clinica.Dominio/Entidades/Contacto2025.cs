using System.Text.RegularExpressions;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;


public readonly record struct ContactoEmail2025(
	string Valor
) : IComoTexto {
	public string ATexto() {
		return Valor;
	}
	public static Result<ContactoEmail2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoEmail2025>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<ContactoEmail2025>.Error("Correo electrónico inválido.");

		return new Result<ContactoEmail2025>.Ok(new(input.Trim()));
	}
}


public readonly record struct ContactoTelefono2025(
	string Valor
) : IComoTexto {
	public string ATexto() {
		return Valor;
	}
	public static Result<ContactoTelefono2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoTelefono2025>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<ContactoTelefono2025>.Error("Teléfono inválido.");

		return new Result<ContactoTelefono2025>.Ok(new ContactoTelefono2025(input.Trim()));
	}
}
public record struct Contacto2025(
	ContactoEmail2025 Email,
	ContactoTelefono2025 Telefono
): IComoTexto {
	public string ATexto() {
		return
			$"Contacto:\n" +
			$"  • Email: {Email.ATexto()}\n" +
			$"  • Teléfono: {Telefono.ATexto()}";
	}
	public static Result<Contacto2025> Crear(Result<ContactoEmail2025> emailResult, Result<ContactoTelefono2025> telResult)
		=> emailResult.Bind(emailOk => telResult.Map(telOk => new Contacto2025(emailOk, telOk)));
}
