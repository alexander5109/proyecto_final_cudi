using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.TiposDeEntidad;

public static class ClinicaNegocio {
	public static readonly HorarioDeAtencion Atencion = new(
		new TimeOnly(8, 0),
		new TimeOnly(19, 0)
	);
}