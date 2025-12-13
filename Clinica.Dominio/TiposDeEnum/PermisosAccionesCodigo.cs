using Clinica.Dominio.TiposDeEntidad;

namespace Clinica.Dominio.TiposDeEnum;


public enum PermisosAccionesCodigo {
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


public static class PermisoSistema {
	private static readonly Dictionary<UsuarioRoleCodigo, HashSet<PermisosAccionesCodigo>> tabla = new() {
		[UsuarioRoleCodigo.Nivel1Superadmin] = [], //TIENE TODOS LOS PERMISOS

		[UsuarioRoleCodigo.Nivel2Administrativo] = [
			PermisosAccionesCodigo.VerPacientes,
			PermisosAccionesCodigo.VerTurnos,
			PermisosAccionesCodigo.VerMedicos,
			PermisosAccionesCodigo.VerHorarios,

			PermisosAccionesCodigo.CrearPacientes,
			PermisosAccionesCodigo.CrearTurnos,
			PermisosAccionesCodigo.CrearMedicos,
			PermisosAccionesCodigo.CrearHorarios,

			PermisosAccionesCodigo.CancelarTurno,
			PermisosAccionesCodigo.ReprogramarTurno,
			PermisosAccionesCodigo.SolicitarTurno,

			PermisosAccionesCodigo.UpdateEntidades,
			PermisosAccionesCodigo.VerUsuarios
            // No borra entidades sensibles

		],

		[UsuarioRoleCodigo.Nivel3Recepcionista] = [
            // Puede operar turnos y datos básicos, pero no estructura
            PermisosAccionesCodigo.VerPacientes,
			PermisosAccionesCodigo.VerTurnos,
			PermisosAccionesCodigo.VerMedicos,
			PermisosAccionesCodigo.VerHorarios,

			PermisosAccionesCodigo.UpdatePacientes,
			PermisosAccionesCodigo.CrearPacientes,
			PermisosAccionesCodigo.CrearTurnos,

			PermisosAccionesCodigo.CancelarTurno,
			PermisosAccionesCodigo.SolicitarTurno,
			PermisosAccionesCodigo.GestionDeTurnos,
			PermisosAccionesCodigo.VerUsuarios,

			PermisosAccionesCodigo.UpdateEntidades // ← Puede modificar pacientes
            // No puede reprogramar turnos (opcional: lo podés habilitar)
            // No puede crear médicos/usuarios/horarios
            // No borra entidades
		],

		[UsuarioRoleCodigo.Nivel4Medico] = [
			PermisosAccionesCodigo.VerPacientes,  // opcional
            PermisosAccionesCodigo.VerTurnos,
			PermisosAccionesCodigo.VerHorarios,
			PermisosAccionesCodigo.SolicitarTurno,
			PermisosAccionesCodigo.VerUsuarios
            // No modifica nada
		],
	};

	public static bool TienePermisosPara(this UsuarioRoleCodigo rol, PermisosAccionesCodigo permiso) => rol is UsuarioRoleCodigo.Nivel1Superadmin || tabla.TryGetValue(rol, out HashSet<PermisosAccionesCodigo>? set) && set.Contains(permiso);
	public static bool HasPermission(this Usuario2025 u, PermisosAccionesCodigo permiso) => TienePermisosPara(u.EnumRole, permiso);
}
