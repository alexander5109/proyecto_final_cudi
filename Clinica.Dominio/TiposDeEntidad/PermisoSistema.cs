using Clinica.Dominio.TiposDeEnum;

namespace Clinica.Dominio.TiposDeEntidad;


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
	UpdateHorarios,
	UpdatePacientes,
	CrearHorarios,
	VerHorarios,


	SolicitarTurno,
	GestionDeTurnos

}


public static class UsuarioPermisosExtensions {
	private static readonly Dictionary<UsuarioEnumRole, HashSet<PermisoSistema>> tabla = new() {
		[UsuarioEnumRole.Nivel1Superadmin] = [
			PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerUsuarios,
			PermisoSistema.VerMedicos,
			PermisoSistema.VerHorarios,

			PermisoSistema.CrearPacientes,
			PermisoSistema.CrearTurnos,
			PermisoSistema.CrearMedicos,
			PermisoSistema.CrearUsuarios,
			PermisoSistema.CrearHorarios,

			PermisoSistema.CancelarTurno,
			PermisoSistema.ReprogramarTurno,
			PermisoSistema.SolicitarTurno,

			PermisoSistema.UpdateEntidades,
			PermisoSistema.DeleteEntidades,
			PermisoSistema.GestionDeTurnos,

		],

		[UsuarioEnumRole.Nivel2Administrativo] = [
			PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerMedicos,
			PermisoSistema.VerHorarios,

			PermisoSistema.CrearPacientes,
			PermisoSistema.CrearTurnos,
			PermisoSistema.CrearMedicos,
			PermisoSistema.CrearHorarios,

			PermisoSistema.CancelarTurno,
			PermisoSistema.ReprogramarTurno,
			PermisoSistema.SolicitarTurno,

			PermisoSistema.UpdateEntidades,
            // No borra entidades sensibles

		],

		[UsuarioEnumRole.Nivel3Secretaria] = [
            // Puede operar turnos y datos básicos, pero no estructura
            PermisoSistema.VerPacientes,
			PermisoSistema.VerTurnos,
			PermisoSistema.VerMedicos,
			PermisoSistema.VerHorarios,

			PermisoSistema.UpdatePacientes,
			PermisoSistema.CrearPacientes,
			PermisoSistema.CrearTurnos,

			PermisoSistema.CancelarTurno,
			PermisoSistema.SolicitarTurno,
			PermisoSistema.GestionDeTurnos,

			PermisoSistema.UpdateEntidades, // ← Puede modificar pacientes
            // No puede reprogramar turnos (opcional: lo podés habilitar)
            // No puede crear médicos/usuarios/horarios
            // No borra entidades
		],

		[UsuarioEnumRole.Nivel4Medico] = [
			PermisoSistema.VerPacientes,  // opcional
            PermisoSistema.VerTurnos,
			PermisoSistema.VerHorarios,
			PermisoSistema.SolicitarTurno
            // No modifica nada
		],
	};

	public static bool Tiene(UsuarioEnumRole rol, PermisoSistema permiso) =>
		tabla.TryGetValue(rol, out HashSet<PermisoSistema>? set) && set.Contains(permiso);
	public static bool HasPermission(this Usuario2025 u, PermisoSistema permiso)
		=> Tiene(u.EnumRole, permiso);
}
