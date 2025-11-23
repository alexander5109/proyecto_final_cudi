using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.ListasOrganizadoras;

public static class DisponibilidadesExtensions {
	public static Result<DisponibilidadEspecialidad2025> TomarPrimera(this Result<IReadOnlyList<DisponibilidadEspecialidad2025>> listadoResult) {
		return listadoResult.Match<Result<DisponibilidadEspecialidad2025>>(
			ok => {
				// la lista existe, ahora chequeamos si tiene elementos
				if (ok.Count == 0)
					return new Result<DisponibilidadEspecialidad2025>.Error(
						"No hay disponibilidades para seleccionar."
					);
				return new Result<DisponibilidadEspecialidad2025>.Ok(ok[0]);
			},
			mensajeError =>
				new Result<DisponibilidadEspecialidad2025>.Error(mensajeError)
		);
	}

	public static Result<IReadOnlyList<DisponibilidadEspecialidad2025>> AplicarFiltrosOpcionales(this Result<IReadOnlyList<DisponibilidadEspecialidad2025>> disponibilidadesResult, SolicitudDeTurnoPreferencias preferencias) {
		if (disponibilidadesResult is Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error err)
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error(err.Mensaje);

		IReadOnlyList<DisponibilidadEspecialidad2025> lista = ((Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok)disponibilidadesResult).Valor;
		IEnumerable<DisponibilidadEspecialidad2025> filtradas = lista;
		if (preferencias.DiaPreferido is DiaSemana2025 dia)
			filtradas = filtradas.Where(d => d.FechaHoraDesde.DayOfWeek == dia.Valor);

		if (preferencias.MomentoPreferido is TardeOMañana momento)
			filtradas = filtradas.Where(d => momento.AplicaA(d.FechaHoraDesde));

		if (filtradas.Any()) {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Ok([.. filtradas]);
		} else {
			return new Result<IReadOnlyList<DisponibilidadEspecialidad2025>>.Error("No se encontraron disponibilidades");
		}
	}

}
