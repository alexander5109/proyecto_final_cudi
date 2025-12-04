public enum PermisoSistema {
	VerPacientes,
	VerTurnos,
	VerMedicos,
	VerUsuarios,
	CrearPacientes,
	CrearMedicos,
	CrearTurnos,
	CrearUsuarios,
	CancelarTurno,
	ReprogramarTurno,

	DeleteEntidades,
	UpdateEntidades,
	CrearHorarios,
	VerHorarios,


	SolicitarTurno

	// más adelante: editar paciente, etc.
}


public static class PermisosPorRol {
	private static readonly Dictionary<UsuarioEnumRole, HashSet<PermisoSistema>> tabla = new() {
		[UsuarioEnumRole.Nivel1Admin] = [
			PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerUsuarios,
			PermisoSistema.VerMedicos,
			PermisoSistema.CrearTurnos,
			PermisoSistema.CancelarTurno,
			PermisoSistema.ReprogramarTurno,
			PermisoSistema.CrearPacientes,
			PermisoSistema.UpdateEntidades,
			PermisoSistema.CrearMedicos,
			PermisoSistema.DeleteEntidades,
			PermisoSistema.CrearUsuarios,
			PermisoSistema.CrearHorarios,

		],

		[UsuarioEnumRole.Nivel2Secretaria] = [
			PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerMedicos,
			PermisoSistema.CrearTurnos,
			PermisoSistema.CancelarTurno,
			PermisoSistema.CrearPacientes,
			PermisoSistema.UpdateEntidades,
			PermisoSistema.CrearHorarios,
			PermisoSistema.VerHorarios,
			PermisoSistema.SolicitarTurno

		],

		[UsuarioEnumRole.Nivel3Medico] = [
			PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerMedicos,
			PermisoSistema.VerHorarios,
			PermisoSistema.SolicitarTurno
		],

		[UsuarioEnumRole.Nivel4Paciente] = [
			PermisoSistema.VerHorarios,
			PermisoSistema.SolicitarTurno
		],
	};

	public static bool Tiene(UsuarioEnumRole rol, PermisoSistema permiso) =>
		tabla.TryGetValue(rol, out HashSet<PermisoSistema>? set) && set.Contains(permiso);
}


public static class UsuarioPermisosExtensions {
	public static bool HasPermission(this Usuario2025 u, PermisoSistema permiso)
		=> PermisosPorRol.Tiene(u.EnumRole, permiso);
}
