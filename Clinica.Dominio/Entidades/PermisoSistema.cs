public enum PermisoSistema {
	VerPacientes,
	VerTurnos,
	VerMedicos,
	CrearPacientes,

	CrearTurno,
	CancelarTurno,
	ReprogramarTurno,
	EliminarEntidad,
	EditarPacientes

	// más adelante: editar paciente, etc.
}


public static class PermisosPorRol {
	private static readonly Dictionary<UsuarioEnumRole, HashSet<PermisoSistema>> tabla = new() {
		[UsuarioEnumRole.Nivel1Admin] = [
            PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerMedicos,
			PermisoSistema.CrearTurno,
			PermisoSistema.CancelarTurno,
			PermisoSistema.ReprogramarTurno,
			PermisoSistema.CrearPacientes,
			PermisoSistema.EditarPacientes,
			PermisoSistema.EliminarEntidad
		],

		[UsuarioEnumRole.Nivel2Secretaria] = [
            PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerMedicos,
			PermisoSistema.CrearTurno,
			PermisoSistema.CancelarTurno,
			PermisoSistema.CrearPacientes,
			PermisoSistema.EditarPacientes,

		],

		[UsuarioEnumRole.Nivel3Medico] = [
            PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerMedicos,
		],

		[UsuarioEnumRole.Nivel4Paciente] = [
            PermisoSistema.VerTurnos,
		],
	};

	public static bool Tiene(UsuarioEnumRole rol, PermisoSistema permiso) =>
		tabla.TryGetValue(rol, out var set) && set.Contains(permiso);
}


public static class UsuarioPermisosExtensions {
	public static bool HasPermission(this Usuario2025 u, PermisoSistema permiso)
		=> PermisosPorRol.Tiene(u.EnumRole, permiso);
}
