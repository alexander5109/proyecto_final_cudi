using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;

namespace Clinica.Dominio.TiposDeAgregado;


public record HorarioFranja2026(
   DayOfWeek DiaSemana,
   TimeOnly HoraDesde,
   TimeOnly HoraHasta,
   DateOnly VigenteDesde,
   DateOnly? VigenteHasta
) {
	//public static HorarioFranja2026 Crear(
	//	DayOfWeek dia,
	//	TimeOnly desde,
	//	TimeOnly hasta,
	//	DateOnly vigenteDesde,
	//	DateOnly? vigenteHasta
	//) {
	//	return new HorarioFranja2026(dia, desde, hasta, vigenteDesde, vigenteHasta);
	//}

	public bool SeSolapaCon(HorarioFranja2026 other) {
		if (DiaSemana != other.DiaSemana)
			return false;

		// intersección de vigencias
		var desde = VigenteDesde > other.VigenteDesde
			? VigenteDesde
			: other.VigenteDesde;

		var hasta = MinNullable(VigenteHasta, other.VigenteHasta);

		if (hasta is not null && desde > hasta)
			return false;

		// solape horario
		return HoraDesde < other.HoraHasta
			&& HoraHasta > other.HoraDesde;
	}

	private static DateOnly? MinNullable(DateOnly? a, DateOnly? b)
		=> a is null ? b
		 : b is null ? a
		 : a < b ? a : b;




	public static Result<HorarioFranja2026> CrearResult(
		//MedicoId medicoId,
		DayOfWeek dia,
		TimeOnly desde,
		TimeOnly hasta,
		DateOnly vigenteDesde,
		DateOnly? vigenteHasta
	) {
		// -----------------------------
		// VALIDACIONES TEMPORALES
		// -----------------------------

		if (desde >= hasta)
			return new Result<HorarioFranja2026>.Error(
				"Error en horario {dia.ATexto()} {desde}-{hasta}: \n   La hora de inicio debe ser anterior a la hora de fin."
			);

		if (vigenteDesde >= vigenteHasta)
			return new Result<HorarioFranja2026>.Error(
				"Error en horario {dia.ATexto()} {desde}-{hasta} vigente entre {vigenteDesde}{vigenteHasta}: \n   La fecha de inicio de vigencia debe ser anterior a la fecha de fin."
			);

		// -----------------------------
		// REGLAS DE NEGOCIO (CLÍNICA)
		// -----------------------------
		var atencion = ClinicaNegocio.Atencion;

		if (desde < atencion.DesdeHs || hasta > atencion.HastaHs)
			return new Result<HorarioFranja2026>.Error(
				$"El horario {desde}-{hasta} del {dia.ATexto()} debe estar dentro del horario de atención de la clínica " +
				$"({atencion.DesdeHs} – {atencion.HastaHs})."
			);

		return new Result<HorarioFranja2026>.Ok(
			new HorarioFranja2026(dia, desde, hasta, vigenteDesde, vigenteHasta)
		);
	}
}



public readonly record struct HorariosMedicos2026Agg(
	MedicoId MedicoId,
	IReadOnlyCollection<HorarioFranja2026> Franjas
) {
	public static Result<HorariosMedicos2026Agg> CrearResult(
		MedicoId medicoId,
		IReadOnlyCollection<HorarioFranja2026> franjas
	) {
		List<HorarioFranja2026> lista = [.. franjas];

		for (int i = 0; i < lista.Count; i++) {
			for (int j = i + 1; j < lista.Count; j++) {
				if (lista[i].SeSolapaCon(lista[j])) {
					return new Result<HorariosMedicos2026Agg>.Error(
						$"Solapamiento detectado: \n {lista[i]}\n solapa con:\n{lista[j]}"
					);
				}
			}
		}
		return new Result<HorariosMedicos2026Agg>.Ok(new HorariosMedicos2026Agg(medicoId, lista));
	}
}





/*
public readonly record struct Horario2025(
   MedicoId MedicoId,
   DayOfWeek DiaSemana,
   TimeOnly HoraDesde,
   TimeOnly HoraHasta,
   DateOnly VigenteDesde,
   DateOnly VigenteHasta
) : IComoTexto {
	public string ATexto()
		=> $"{DiaSemana.ATexto()}: {HoraDesde.ATextoHoras()} — {HoraHasta.ATextoHoras()} (vigencia {VigenteDesde.ATexto()} → {VigenteHasta.ATexto()}";

	public static Horario2025 Crear(
		MedicoId medicoId,
		DayOfWeek dia,
		TimeOnly desde,
		TimeOnly hasta,
		DateOnly vigenteDesde,
		DateOnly vigenteHasta
	) {
		return new Horario2025(medicoId, dia, desde, hasta, vigenteDesde, vigenteHasta);
	}


	public static Result<Horario2025> CrearResult(
		MedicoId medicoId,
		DayOfWeek dia,
		TimeOnly desde,
		TimeOnly hasta,
		DateOnly vigenteDesde,
		DateOnly vigenteHasta
	) {
		// -----------------------------
		// VALIDACIONES TEMPORALES
		// -----------------------------
		if (desde >= hasta)
			return new Result<Horario2025>.Error(
				"La hora de inicio debe ser anterior a la hora de fin."
			);

		if (vigenteDesde >= vigenteHasta)
			return new Result<Horario2025>.Error(
				"La fecha de inicio de vigencia debe ser anterior a la fecha de fin."
			);

		// -----------------------------
		// REGLAS DE NEGOCIO (CLÍNICA)
		// -----------------------------
		var atencion = ClinicaNegocio.Atencion;

		if (desde < atencion.DesdeHs || hasta > atencion.HastaHs)
			return new Result<Horario2025>.Error(
				$"El horario debe estar dentro del horario de atención de la clínica " +
				$"({atencion.DesdeHs} – {atencion.HastaHs})."
			);

		return new Result<Horario2025>.Ok(
			new Horario2025(medicoId, dia, desde, hasta, vigenteDesde, vigenteHasta)
		);
	}
}
*/



//public readonly record HorariosMedicos2026Agg(
//	HorarioId Id,
//	MedicoId MedicoId,
//	HorarioFranja2026 Horario
//);
//public readonly record struct HorariosMedicos2026Agg(HorarioId Id, HorarioFranja2026 Horario) {
//	public static HorariosMedicos2026Agg Crear(HorarioId id, HorarioFranja2026 turno) => new(id, turno);
//	public static Result<HorariosMedicos2026Agg> CrearResult(
//		Result<HorarioId> idResult,
//		Result<HorarioFranja2026> pacienteResult
//	)
//		=> from id in idResult
//		   from paciente in pacienteResult
//		   select new HorariosMedicos2026Agg(
//			   id,
//			   paciente
//		   );
//}
/*

public readonly record struct HorariosMedicos2026Agg(
	IReadOnlyList<HorarioFranja2026> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores.Count == 0)
			return "Lista de horarios: (vacía)";
		IEnumerable<string> lineas = Valores
			.Select(v => "- " + v.ATexto());
		return "Lista de horarios:\n" + string.Join("\n", lineas);
	}

	public static Result<HorariosMedicos2026Agg> CrearResult(IEnumerable<HorarioFranja2026>? horariosInput) {
		if (horariosInput is null)
			return new Result<HorariosMedicos2026Agg>.Error("La lista de horarios no puede ser nula.");

        // Materializamos
        List<HorarioFranja2026> lista = [.. horariosInput];

		if (lista.Count == 0)
			return new Result<HorariosMedicos2026Agg>.Error("La lista de horarios no puede estar vacía.");

        // VALIDACION INDIVIDUAL
        List<string> errores = [];

		foreach (HorarioFranja2026 h in lista) {
			if (h.HoraDesde >= h.HoraHasta)
				errores.Add($"El horario {h} tiene un rango horario inválido: Desde debe ser < Hasta.");

			if (h.VigenteDesde > h.VigenteHasta)
				errores.Add($"El horario {h} tiene un rango de vigencia inválido: VigenteDesde debe ser <= VigenteHasta.");
		}

		if (errores.Count > 0)
			return new Result<HorariosMedicos2026Agg>.Error(string.Join("\n", errores));

        // VALIDACION DE SUPERPOSICIONES
        // Agrupar por médico y día
        IEnumerable<IGrouping<(MedicoId MedicoId, DayOfWeek DiaSemana), HorarioFranja2026>> grupos = lista
			.GroupBy(h => (h.MedicoId, h.DiaSemana));

		foreach (IGrouping<(MedicoId MedicoId, DayOfWeek DiaSemana), HorarioFranja2026> g in grupos) {
            List<HorarioFranja2026> horarios = [.. g.OrderBy(h => h.HoraDesde)];

			for (int i = 0; i < horarios.Count - 1; i++) {
                HorarioFranja2026 actual = horarios[i];
                HorarioFranja2026 siguiente = horarios[i + 1];

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
			return new Result<HorariosMedicos2026Agg>.Error(string.Join("\n", errores));

		// Todo ok
		return new Result<HorariosMedicos2026Agg>.Ok(
			new HorariosMedicos2026Agg(lista)
		);
	}




}




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
		List<Horario2025> lista = [.. horariosInput];

		if (lista.Count == 0)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de horarios no puede estar vacía.");

		// VALIDACION INDIVIDUAL
		List<string> errores = [];

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
			List<Horario2025> horarios = [.. g.OrderBy(h => h.HoraDesde)];

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

*/