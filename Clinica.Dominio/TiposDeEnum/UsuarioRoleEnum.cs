using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeEnum;

public enum UsuarioRoleEnum : byte {
	Nivel1Superadmin = 1,
	Nivel2Administrativo = 2,
	Nivel3Recepcionista = 3,
	Nivel4Medico = 4
}