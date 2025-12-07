namespace Clinica.Dominio.Entidades;

public record HorarioDeAtencion(TimeOnly DesdeHs, TimeOnly HastaHs);

public static class ClinicaNegocio {
	public static readonly HorarioDeAtencion Atencion = new(
		new TimeOnly(8, 0),
		new TimeOnly(19, 0)
	);
}