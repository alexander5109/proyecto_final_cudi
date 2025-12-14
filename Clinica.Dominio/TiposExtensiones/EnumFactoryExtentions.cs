using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;

namespace Clinica.Dominio.TiposExtensiones;

public static class EnumFactoryExtentions {

	public static Result<TurnoEstadoEnum> CrearResult(this TurnoEstadoEnum codigo) {
		if (!Enum.IsDefined(typeof(TurnoEstadoEnum), codigo)) {
			return new Result<TurnoEstadoEnum>.Error($"Valor de TurnoEstadoEnum inválido: {codigo}");
		}
		return new Result<TurnoEstadoEnum>.Ok(codigo);
	}


	public static Result<UsuarioRoleEnum> CrearResult(this UsuarioRoleEnum raw) =>
		raw switch {
			UsuarioRoleEnum.Nivel1Superadmin
			or UsuarioRoleEnum.Nivel2Administrativo
			or UsuarioRoleEnum.Nivel3Recepcionista
			or UsuarioRoleEnum.Nivel4Medico
				=> new Result<UsuarioRoleEnum>.Ok(raw),

			_ => new Result<UsuarioRoleEnum>.Error($"El rol '{raw}' no es válido.")
		};
}
