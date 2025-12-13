using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Dominio.TiposDeEntidad;

public readonly record struct ListaHorarioMedicos2025(
	IReadOnlyList<Horario2025> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores.Count == 0)
			return "Lista de horarios: (vacía)";
		IEnumerable<string> lineas = Valores
			.Select(v => "- " + v.ATexto());
		return "Lista de horarios:\n" + string.Join("\n", lineas);
	}

	public static Result<ListaHorarioMedicos2025> CrearResult(IEnumerable<Horario2025>? horariosInput) {
		if (horariosInput is null)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de horarios no puede ser nula.");

        // Materializamos
        List<Horario2025> lista = horariosInput.ToList();

		if (lista.Count == 0)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de horarios no puede estar vacía.");

        // VALIDACION INDIVIDUAL
        List<string> errores = new List<string>();

		foreach (Horario2025 h in lista) {
			if (h.HoraDesde >= h.HoraHasta)
				errores.Add($"El horario {h} tiene un rango horario inválido: Desde debe ser < Hasta.");

			if (h.VigenteDesde > h.VigenteHasta)
				errores.Add($"El horario {h} tiene un rango de vigencia inválido: VigenteDesde debe ser <= VigenteHasta.");
		}

		if (errores.Count > 0)
			return new Result<ListaHorarioMedicos2025>.Error(string.Join("\n", errores));

        // VALIDACION DE SUPERPOSICIONES
        // Agrupar por médico y día
        IEnumerable<IGrouping<(MedicoId MedicoId, DayOfWeek DiaSemana), Horario2025>> grupos = lista
			.GroupBy(h => (h.MedicoId, h.DiaSemana));

		foreach (IGrouping<(MedicoId MedicoId, DayOfWeek DiaSemana), Horario2025> g in grupos) {
            List<Horario2025> horarios = g
				.OrderBy(h => h.HoraDesde)
				.ToList();

			for (int i = 0; i < horarios.Count - 1; i++) {
                Horario2025 actual = horarios[i];
                Horario2025 siguiente = horarios[i + 1];

				// Superposición si actual termina después de que empieza el siguiente
				if (actual.HoraHasta > siguiente.HoraDesde) {
					errores.Add(
						$"Superposición detectada para el médico {actual.MedicoId} " +
						$"el día {actual.DiaSemana}: " +
						$"({actual.HoraDesde} - {actual.HoraHasta}) se solapa con " +
						$"({siguiente.HoraDesde} - {siguiente.HoraHasta})."
					);
				}
			}
		}

		if (errores.Count > 0)
			return new Result<ListaHorarioMedicos2025>.Error(string.Join("\n", errores));

		// Todo ok
		return new Result<ListaHorarioMedicos2025>.Ok(
			new ListaHorarioMedicos2025(lista)
		);
	}




}
