using System;
using System.Collections.Generic;
using System.Text;

namespace Clinica.Dominio.Entidades;

public readonly record struct UsuarioId(int Valor);
public readonly record struct NombreUsuario(string Valor);
public readonly record struct PasswordHasheado(string Valor);
public abstract record UsuarioBase2025(
	UsuarioId UserId,
	NombreUsuario UserName,
	PasswordHasheado UserPassword
);
public sealed record Usuario2025Nivel1Admin(
	UsuarioId UserId,
	NombreUsuario UserName,
	PasswordHasheado UserPassword
) : UsuarioBase2025(UserId, UserName, UserPassword);

public sealed record Usuario2025Nivel2Secretaria(
	UsuarioId UserId,
	NombreUsuario UserName,
	PasswordHasheado UserPassword
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
