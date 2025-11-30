using System;
using System.Collections.Generic;
using System.Text;
using Clinica.Dominio.Entidades;

namespace Clinica.Shared.Dtos;

public static partial class DomainDtos {
	public record UsuarioDto(
	int Id,
	string NombreUsuario,
	string PasswordHash,
	byte EnumRole);
	public static UsuarioBase2025 ToDomain(this UsuarioDto dto) {
        UsuarioId id = new(dto.Id);
        NombreUsuario nombre = new(dto.NombreUsuario);
        ContraseñaHasheada password = new(dto.PasswordHash);

		return dto.EnumRole switch {
			1 => new Usuario2025Nivel1Admin(id, nombre, password),
			2 => new Usuario2025Nivel2Secretaria(id, nombre, password),
			_ => throw new Exception($"Rol desconocido: {dto.EnumRole}")
		};
	}
	public static UsuarioDto ToDto(this UsuarioBase2025 entidad) {
		byte enumrole = entidad switch {
			Usuario2025Nivel1Admin => 1,
			Usuario2025Nivel2Secretaria => 2,
			_ => throw new Exception($"Entidad de dominio todavia no reconocida por infrastructura")
		};
		return new UsuarioDto(entidad.UserId.Valor, entidad.UserName.Valor, entidad.UserPassword.Valor, enumrole);

	}
}