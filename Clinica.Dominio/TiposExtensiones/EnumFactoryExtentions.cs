using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;

namespace Clinica.Dominio.TiposExtensiones;

public static class EnumFactoryExtentions {

	public static Result<TurnoEstadoCodigo> CrearResult(this TurnoEstadoCodigo codigo) {
		if (!Enum.IsDefined(typeof(TurnoEstadoCodigo), codigo)) {
			return new Result<TurnoEstadoCodigo>.Error($"Valor de TurnoEstadoCodigo inválido: {codigo}");
		}
		return new Result<TurnoEstadoCodigo>.Ok(codigo);
	}


	public static Result<UsuarioRoleCodigo> CrearResult(this UsuarioRoleCodigo raw) =>
		raw switch {
			UsuarioRoleCodigo.Nivel1Superadmin
			or UsuarioRoleCodigo.Nivel2Administrativo
			or UsuarioRoleCodigo.Nivel3Secretaria
			or UsuarioRoleCodigo.Nivel4Medico
				=> new Result<UsuarioRoleCodigo>.Ok(raw),

			_ => new Result<UsuarioRoleCodigo>.Error($"El rol '{raw}' no es válido.")
		};
}
