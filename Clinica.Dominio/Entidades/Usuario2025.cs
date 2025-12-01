using System;
using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.Comun;
using static Clinica.Dominio.Dtos.DomainDtos;

namespace Clinica.Dominio.Entidades;


public enum UsuarioEnumRole : byte {
	Nivel1Admin = 1,
	Nivel2Secretaria = 2,
	Nivel3Medico = 3,
	Nivel4Paciente = 4,
}

public readonly record struct UsuarioId(int Valor) {
	public static Result<UsuarioId> Crear(int? id) =>
		id is int idGood
		? new Result<UsuarioId>.Ok(new UsuarioId(idGood))
		: new Result<UsuarioId>.Error("El id no puede ser nulo.");

	public override string ToString() {
		return Valor.ToString();
	}

}
public readonly record struct NombreUsuario(string Valor);
public readonly record struct ContraseñaHasheada(string Valor) {
	public static ContraseñaHasheada CrearFromRaw(string raw) {
		var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		return new(Convert.ToHexString(hash));
	}

	public bool IgualA(string raw) {
		byte[] rawHash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		byte[] stored = Convert.FromHexString(Valor);
		return CryptographicOperations.FixedTimeEquals(rawHash, stored);
	}
}

public abstract record UsuarioBase2025(
	UsuarioId UserId,
	NombreUsuario UserName,
	ContraseñaHasheada UserPassword,
	UsuarioEnumRole EnumRole
) {
	public Result<UsuarioBase2025> PasswordMatch(string raw) {
		return UserPassword.IgualA(raw)
			? new Result<UsuarioBase2025>.Ok(this)
			: new Result<UsuarioBase2025>.Error("Usuario o contraseña incorrectos");
	}

	public static Result<UsuarioBase2025> Crear(UsuarioId id, string nombreUsuario, string passwordHash, UsuarioEnumRole enumrole ) {
		NombreUsuario nombre = new(nombreUsuario);
		ContraseñaHasheada password = new(passwordHash);

		return enumrole switch {
			UsuarioEnumRole.Nivel1Admin =>new Result<UsuarioBase2025>.Ok(new Usuario2025Nivel1Admin(id, nombre, password, enumrole)),
			UsuarioEnumRole.Nivel2Secretaria => new Result<UsuarioBase2025>.Ok(new Usuario2025Nivel2Secretaria(id, nombre, password, enumrole)),
			UsuarioEnumRole.Nivel3Medico => new Result<UsuarioBase2025>.Ok(new Usuario2025Nivel2Secretaria(id, nombre, password, enumrole)),
			UsuarioEnumRole.Nivel4Paciente => new Result<UsuarioBase2025>.Ok(new Usuario2025Nivel2Secretaria(id, nombre, password, enumrole)),
			_ => new Result<UsuarioBase2025>.Error($"Rol desconocido: {enumrole}")
		};
	}
}

public sealed record Usuario2025Nivel1Admin(
	UsuarioId UserId,
	NombreUsuario UserName,
	ContraseñaHasheada UserPassword,
	UsuarioEnumRole EnumRole
) : UsuarioBase2025(UserId, UserName, UserPassword, EnumRole);

public sealed record Usuario2025Nivel2Secretaria(
	UsuarioId UserId,
	NombreUsuario UserName,
	ContraseñaHasheada UserPassword,
	UsuarioEnumRole EnumRole
) : UsuarioBase2025(UserId, UserName, UserPassword, EnumRole);



//public sealed record UsuarioNivel3Medico(
//	UsuarioId UserId,
//	NombreUsuario UserName,
//	MedicoId MedicoRelacionado
//) : UsuarioBase2025(UserId, UserName);

//public sealed record UsuarioNivel4Paciente(
//	UsuarioId UserId,
//	NombreUsuario UserName,
//	PacienteId PacienteRelacionado
//) : UsuarioBase2025(UserId, UserName);
