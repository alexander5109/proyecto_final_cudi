using System;
using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;

public readonly record struct UsuarioId(int Valor);
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
	ContraseñaHasheada UserPassword) {
	public Result<UsuarioBase2025> PasswordMatch(string raw) {
		return UserPassword.IgualA(raw)
			? new Result<UsuarioBase2025>.Ok(this)
			: new Result<UsuarioBase2025>.Error("Usuario o contraseña incorrectos");
	}
}

public sealed record Usuario2025Nivel1Admin(
	UsuarioId UserId,
	NombreUsuario UserName,
	ContraseñaHasheada UserPassword
) : UsuarioBase2025(UserId, UserName, UserPassword);

public sealed record Usuario2025Nivel2Secretaria(
	UsuarioId UserId,
	NombreUsuario UserName,
	ContraseñaHasheada UserPassword
) : UsuarioBase2025(UserId, UserName, UserPassword);



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
