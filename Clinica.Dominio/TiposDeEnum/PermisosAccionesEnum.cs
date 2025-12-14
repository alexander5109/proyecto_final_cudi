using Clinica.Dominio.TiposDeEntidad;

namespace Clinica.Dominio.TiposDeEnum;


public enum PermisosAccionesEnum {
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
	private static readonly Dictionary<UsuarioRoleEnum, HashSet<PermisosAccionesEnum>> tabla = new() {
		[UsuarioRoleEnum.Nivel1Superadmin] = [], //TIENE TODOS LOS PERMISOS

		[UsuarioRoleEnum.Nivel2Administrativo] = [
			PermisosAccionesEnum.VerPacientes,
			PermisosAccionesEnum.VerTurnos,
			PermisosAccionesEnum.VerMedicos,
			PermisosAccionesEnum.VerHorarios,

			PermisosAccionesEnum.CrearPacientes,
			PermisosAccionesEnum.CrearTurnos,
			PermisosAccionesEnum.CrearMedicos,
			PermisosAccionesEnum.CrearHorarios,

			PermisosAccionesEnum.CancelarTurno,
			PermisosAccionesEnum.ReprogramarTurno,
			PermisosAccionesEnum.SolicitarTurno,

			PermisosAccionesEnum.UpdateEntidades,
			PermisosAccionesEnum.VerUsuarios
            // No borra entidades sensibles

		],

		[UsuarioRoleEnum.Nivel3Recepcionista] = [
            // Puede operar turnos y datos básicos, pero no estructura
            PermisosAccionesEnum.VerPacientes,
			PermisosAccionesEnum.VerTurnos,
			PermisosAccionesEnum.VerMedicos,
			PermisosAccionesEnum.VerHorarios,

			PermisosAccionesEnum.UpdatePacientes,
			PermisosAccionesEnum.CrearPacientes,
			PermisosAccionesEnum.CrearTurnos,

			PermisosAccionesEnum.CancelarTurno,
			PermisosAccionesEnum.SolicitarTurno,
			PermisosAccionesEnum.GestionDeTurnos,
			PermisosAccionesEnum.VerUsuarios,

			PermisosAccionesEnum.UpdateEntidades // ← Puede modificar pacientes
            // No puede reprogramar turnos (opcional: lo podés habilitar)
            // No puede crear médicos/usuarios/horarios
            // No borra entidades
		],

		[UsuarioRoleEnum.Nivel4Medico] = [
			PermisosAccionesEnum.VerPacientes,  // opcional
            PermisosAccionesEnum.VerTurnos,
			PermisosAccionesEnum.VerHorarios,
			PermisosAccionesEnum.SolicitarTurno,
			PermisosAccionesEnum.VerUsuarios
            // No modifica nada
		],
	};

	public static bool TienePermisosPara(this UsuarioRoleEnum rol, PermisosAccionesEnum permiso) => rol is UsuarioRoleEnum.Nivel1Superadmin || tabla.TryGetValue(rol, out HashSet<PermisosAccionesEnum>? set) && set.Contains(permiso);
	public static bool HasPermission(this Usuario2025 u, PermisosAccionesEnum permiso) => TienePermisosPara(u.EnumRole, permiso);
}
