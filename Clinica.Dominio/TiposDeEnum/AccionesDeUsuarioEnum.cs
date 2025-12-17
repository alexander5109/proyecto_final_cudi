namespace Clinica.Dominio.TiposDeEnum;


public enum AccionesDeUsuarioEnum {
	VerAtenciones,
	CrearAtenciones,
	ModificarAtenciones,
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
			AccionesDeUsuarioEnum.ModificarHorarios,


			AccionesDeUsuarioEnum.ModificarEntidades,
			AccionesDeUsuarioEnum.VerUsuarios

		],

		[UsuarioRoleEnum.Nivel3Recepcionista] = [
            AccionesDeUsuarioEnum.VerPacientes,
			AccionesDeUsuarioEnum.VerTurnos,
			AccionesDeUsuarioEnum.VerMedicos,
			AccionesDeUsuarioEnum.VerHorarios,

			AccionesDeUsuarioEnum.ModificarPacientes,
			AccionesDeUsuarioEnum.CrearPacientes,


			AccionesDeUsuarioEnum.ProgramarTurno,
			AccionesDeUsuarioEnum.SolicitarTurno,
			AccionesDeUsuarioEnum.ConcretarTurno,
			AccionesDeUsuarioEnum.ConcretarComoAusente,
			AccionesDeUsuarioEnum.CancelarTurno,
			AccionesDeUsuarioEnum.ReprogramarTurno,



			AccionesDeUsuarioEnum.VerUsuarios,

			AccionesDeUsuarioEnum.ModificarEntidades 
		],

		[UsuarioRoleEnum.Nivel4Medico] = [
			AccionesDeUsuarioEnum.ModificarAtenciones,
			AccionesDeUsuarioEnum.CrearAtenciones,
			AccionesDeUsuarioEnum.VerAtenciones,
			AccionesDeUsuarioEnum.VerPacientes,
            AccionesDeUsuarioEnum.VerTurnos,
            AccionesDeUsuarioEnum.VerMedicos,
			AccionesDeUsuarioEnum.VerHorarios,
			AccionesDeUsuarioEnum.SolicitarTurno,
			AccionesDeUsuarioEnum.VerUsuarios
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
