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

	public static Result<UsuarioEnumRole> CrearResult(this UsuarioEnumRole raw) =>
		raw switch {
			UsuarioEnumRole.Nivel1Superadmin
			or UsuarioEnumRole.Nivel2Administrativo
			or UsuarioEnumRole.Nivel3Secretaria
			or UsuarioEnumRole.Nivel4Medico
				=> new Result<UsuarioEnumRole>.Ok(raw),

			_ => new Result<UsuarioEnumRole>.Error($"El rol '{raw}' no es válido.")
		};
}
