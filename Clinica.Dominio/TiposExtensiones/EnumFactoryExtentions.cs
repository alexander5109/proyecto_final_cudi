using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposExtensiones;

public static class EnumFactoryExtentions {

	//public static Result<TurnoEstadoEnum> CrearResult(this TurnoEstadoEnum codigo) {
	//	if (!Enum.IsDefined(typeof(TurnoEstadoEnum), codigo)) {
	//		return new Result<TurnoEstadoEnum>.Error($"Valor de TurnoEstadoEnum inválido: {codigo}");
	//	}
	//	return new Result<TurnoEstadoEnum>.Ok(codigo);
	//}

	public static Result<TEnum> CrearResult<TEnum>(this TEnum? value)
		where TEnum : struct, Enum {
		if (value is not TEnum validEnum) {
			return new Result<TEnum>.Error(
				$"'{typeof(TEnum).Name}' no puede estar vacío"
			);
		}
		if (!Enum.IsDefined(validEnum)) {
			return new Result<TEnum>.Error(
				$"El valor '{value}' no está definido en {typeof(TEnum).Name}."
			);
		}

		return new Result<TEnum>.Ok(validEnum);
	}

	public static Result<TEnum> CrearResult<TEnum>(this TEnum value)
		where TEnum : struct, Enum {
		if (!Enum.IsDefined(value)) {
			return new Result<TEnum>.Error(
				$"El valor '{value}' no está definido en {typeof(TEnum).Name}."
			);
		}

		return new Result<TEnum>.Ok(value);
	}



	//public static class UsuarioRole {
	//	public static Result<UsuarioRoleEnum> CrearResult(UsuarioRoleEnum value) {
	//		if (!Enum.IsDefined(typeof(UsuarioRoleEnum), value)) {
	//			return Result.Fail<UsuarioRoleEnum>(
	//				$"El rol de usuario '{value}' no es válido."
	//			);
	//		}

	//		return Result.Ok(value);
	//	}
	//}

	//public static Result<UsuarioRoleEnum> CrearResult(this UsuarioRoleEnum raw) =>
	//	raw switch {
	//		UsuarioRoleEnum.Nivel1Superadmin
	//		or UsuarioRoleEnum.Nivel2Administrativo
	//		or UsuarioRoleEnum.Nivel3Recepcionista
	//		or UsuarioRoleEnum.Nivel4Medico
	//			=> new Result<UsuarioRoleEnum>.Ok(raw),

	//		_ => new Result<UsuarioRoleEnum>.Error($"El rol '{raw}' no es válido.")
	//	};
}
