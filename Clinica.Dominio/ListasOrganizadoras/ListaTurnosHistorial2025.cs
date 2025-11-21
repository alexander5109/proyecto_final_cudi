using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

public record class ListaTurnosHistorial2025(
	List<Turno2025> Valores
) {
	public static ListaTurnosHistorial2025 Crear() => new([]);

	public Result<ListaTurnosHistorial2025> AgendarTurno(Result<Turno2025> turnoResult) {
		return turnoResult.Match<Result<ListaTurnosHistorial2025>>(
			ok => {
				Valores.Add(ok);
				return new Result<ListaTurnosHistorial2025>.Ok(this);
			},
			mensaje => new Result<ListaTurnosHistorial2025>.Error(mensaje)
		);
	}



	public bool DisponibilidadNoColisiona(
		DisponibilidadEspecialidad2025 disp
	) {
		foreach (var turno in Valores) {
			if (turno.MedicoAsignado != disp.Medico) continue;
			if (turno.Especialidad != disp.Especialidad) continue;

			bool solapa =
				turno.FechaHoraDesde < disp.FechaHoraHasta &&
				disp.FechaHoraDesde < turno.FechaHoraHasta;

			if (solapa)
				return false;
		}

		return true;
	}
}
