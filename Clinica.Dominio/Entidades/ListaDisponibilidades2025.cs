using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Entidades;

public sealed record ListaDisponibilidades2025(
	IReadOnlyList<DisponibilidadEspecialidad2025> Valores
) : IComoTexto {
	public string ATexto() =>
		"Lista de disponibilidades:\n" +
		string.Join("\n", Valores.Select(d => "- " + d.ATexto()));

	public static Result<IEnumerable<DisponibilidadEspecialidad2025>> Buscar(
		IEnumerable<Medico2025> medicos,
		DateTime fechaMinima) {
		try {
			var secuencia = GenerarDisponibilidades(medicos, fechaMinima);

			return new Result<IEnumerable<DisponibilidadEspecialidad2025>>
				.Ok(secuencia);
		} catch (Exception ex) {
			return new Result<IEnumerable<DisponibilidadEspecialidad2025>>
				.Error("Error generando disponibilidades: " + ex.Message);
		}
	}

	// Código original intacto
	public static IEnumerable<DisponibilidadEspecialidad2025> GenerarDisponibilidades(
		IEnumerable<Medico2025> medicos,
		DateTime fechaMinima) {
		foreach (var medico in medicos)
			foreach (var especialidad in medico.Especialidades.Valores) {
				int duracion = especialidad.DuracionConsultaMinutos;

				foreach (var franja in medico.ListaHorarios.Valores) {
					DateTime fecha = fechaMinima.Date;
					while (fecha.DayOfWeek != franja.DiaSemana.Valor)
						fecha = fecha.AddDays(1);

					for (int semana = 0; semana < 30; semana++, fecha = fecha.AddDays(7)) {
						DateTime desde = fecha + franja.Desde.Valor.ToTimeSpan();
						DateTime hasta = fecha + franja.Hasta.Valor.ToTimeSpan();

						for (DateTime slot = desde; slot < hasta; slot = slot.AddMinutes(duracion)) {
							yield return new DisponibilidadEspecialidad2025(
								especialidad,
								medico,
								slot,
								slot.AddMinutes(duracion)
							);
						}
					}
				}
			}
	}

	public static bool EsTarde(DateTime dt) => dt.Hour >= 13;
	public static bool EsMañana(DateTime dt) => dt.Hour < 13;
}


public static class DisponibilidadesExtensions {
	public static Result<ListaDisponibilidades2025> ToListaDisponibilidades2025(
		this IEnumerable<DisponibilidadEspecialidad2025> secuencia) {
		var lista = secuencia.ToList();

		if (lista.Count == 0)
			return new Result<ListaDisponibilidades2025>.Error("No se encontraron disponibilidades.");

		return new Result<ListaDisponibilidades2025>.Ok(
			new ListaDisponibilidades2025(lista)
		);
	}
}
