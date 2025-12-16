using Clinica.Dominio.TiposDeEntidad;

namespace Clinica.Dominio.TiposDeEnum;


public enum AccionesDeUsuarioEnum {
	EliminarEntidades,
	EliminarUsuarios,

	ModificarEntidades,
	ModificarHorarios,
	ModificarPacientes,

	Publico,
	VerPacientes,
	VerTurnos,
	VerMedicos,
	VerUsuarios,
	VerHorarios,

	CrearPacientes,
	CrearMedicos,
	CrearTurnos,
	CrearUsuarios,
	CrearHorarios,

	ProgramarTurno,
	CancelarTurno,
	ReprogramarTurno,
	SolicitarTurno,
	ConcretarTurno,
	ConcretarComoAusente,
	//GestionDeTurnosCompleta
}


public static class PermisoSistema {
	public static readonly Dictionary<UsuarioRoleEnum, HashSet<AccionesDeUsuarioEnum>> DiccionarioDeRoles = new() {
		[UsuarioRoleEnum.Nivel1Superadmin] = [], //TIENE TODOS LOS PERMISOS

		[UsuarioRoleEnum.Nivel2Administrativo] = [
			AccionesDeUsuarioEnum.EliminarUsuarios,
			AccionesDeUsuarioEnum.VerPacientes,
			AccionesDeUsuarioEnum.VerTurnos,
			AccionesDeUsuarioEnum.VerMedicos,
			AccionesDeUsuarioEnum.VerHorarios,

			AccionesDeUsuarioEnum.CrearPacientes,
			AccionesDeUsuarioEnum.CrearTurnos,
			AccionesDeUsuarioEnum.CrearMedicos,
			AccionesDeUsuarioEnum.CrearHorarios,
			AccionesDeUsuarioEnum.CrearUsuarios,

			AccionesDeUsuarioEnum.CancelarTurno,
			AccionesDeUsuarioEnum.ReprogramarTurno,
			AccionesDeUsuarioEnum.SolicitarTurno,
			AccionesDeUsuarioEnum.ModificarHorarios,


			AccionesDeUsuarioEnum.ModificarEntidades,
			AccionesDeUsuarioEnum.VerUsuarios
            // No borra entidades sensibles

		],

		[UsuarioRoleEnum.Nivel3Recepcionista] = [
            // Puede operar turnos y datos básicos, pero no estructura
            AccionesDeUsuarioEnum.VerPacientes,
			AccionesDeUsuarioEnum.VerTurnos,
			AccionesDeUsuarioEnum.VerMedicos,
			AccionesDeUsuarioEnum.VerHorarios,

			AccionesDeUsuarioEnum.ModificarPacientes,
			AccionesDeUsuarioEnum.CrearPacientes,


			AccionesDeUsuarioEnum.ProgramarTurno,
			AccionesDeUsuarioEnum.CancelarTurno,
			AccionesDeUsuarioEnum.SolicitarTurno,
			AccionesDeUsuarioEnum.ConcretarTurno,
			AccionesDeUsuarioEnum.ConcretarComoAusente,

			AccionesDeUsuarioEnum.VerUsuarios,

			AccionesDeUsuarioEnum.ModificarEntidades // ← Puede modificar pacientes
            // No puede reprogramar turnos (opcional: lo podés habilitar)
            // No puede crear médicos/usuarios/horarios
            // No borra entidades
		],

		[UsuarioRoleEnum.Nivel4Medico] = [
			AccionesDeUsuarioEnum.VerPacientes,  // opcional
            AccionesDeUsuarioEnum.VerTurnos,
			AccionesDeUsuarioEnum.VerHorarios,
			AccionesDeUsuarioEnum.SolicitarTurno,
			AccionesDeUsuarioEnum.VerUsuarios
            // No modifica nada
		],
	};


	public static bool TienePermisosPara(
		this UsuarioRoleEnum rol,
		AccionesDeUsuarioEnum permiso
	) {
		if (permiso == AccionesDeUsuarioEnum.Publico)
			return true;

		if (rol == UsuarioRoleEnum.Nivel1Superadmin)
			return true;

		return DiccionarioDeRoles.TryGetValue(rol, out var set)
			   && set.Contains(permiso);
	}
	//public static bool HasPermission(this Usuario2025 u, AccionesDeUsuarioEnum permiso) => TienePermisosPara(u.EnumRole, permiso);
}
