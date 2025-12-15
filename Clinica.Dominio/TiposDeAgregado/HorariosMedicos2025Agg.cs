using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.TiposDeAgregado;


public record HorarioFranja2025WithMedicoId(
	MedicoId MedicoId,
	DayOfWeek DiaSemana,
	TimeOnly HoraDesde,
	TimeOnly HoraHasta,
	DateOnly VigenteDesde,
	DateOnly? VigenteHasta
);


public record HorarioFranja2025(
   DayOfWeek DiaSemana,
   TimeOnly HoraDesde,
   TimeOnly HoraHasta,
   DateOnly VigenteDesde,
   DateOnly? VigenteHasta
) {
	//public string ATexto() => $"{DiaSemana.ATexto()}: {HoraDesde.ATextoHoras()} — {HoraHasta.ATextoHoras()} (vigencia {VigenteDesde.ATexto()} → {VigenteHasta.ATexto()}";

	public static HorarioFranja2025 Crear(
		//MedicoId medicoId,
		DayOfWeek dia,
		TimeOnly desde,
		TimeOnly hasta,
		DateOnly vigenteDesde,
		DateOnly? vigenteHasta
	) {
		return new HorarioFranja2025(dia, desde, hasta, vigenteDesde, vigenteHasta);
	}


	public bool SeSolapaCon(HorarioFranja2025 other) {
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


	//public static Result<HorarioFranja2025> CrearResult(
	//	MedicoId medicoId,
	//	DayOfWeek dia,
	//	TimeOnly desde,
	//	TimeOnly hasta,
	//	DateOnly vigenteDesde,
	//	DateOnly vigenteHasta
	//) {
	//	if (desde >= hasta)
	//		return new Result<HorarioFranja2025>.Error("La hora de inicio debe ser anterior a la hora de fin.");

	//	if (vigenteDesde >= vigenteHasta)
	//		return new Result<HorarioFranja2025>.Error("La fecha de inicio de vigencia debe ser anterior a la fecha de fin.");

	//	return new Result<HorarioFranja2025>.Ok(
	//		new HorarioFranja2025(medicoId, dia, desde, hasta, vigenteDesde, vigenteHasta)
	//	);
	//}
}



public readonly record struct  HorariosMedicos2025Agg(MedicoId MedicoId, IReadOnlyCollection<HorarioFranja2025> Franjas) {
	public static Result<HorariosMedicos2025Agg> CrearResult(
		MedicoId medicoId,
		IReadOnlyCollection<HorarioFranja2025> franjas) {
		var lista = franjas.ToList();

		for (int i = 0; i < lista.Count; i++) {
			for (int j = i + 1; j < lista.Count; j++) {
				if (lista[i].SeSolapaCon(lista[j])) {
					return new Result<HorariosMedicos2025Agg>.Error(
						$"Solapamiento detectado: {lista[i]} / {lista[j]}"
					);
				}
			}
		}
		return new Result<HorariosMedicos2025Agg>.Ok(new HorariosMedicos2025Agg(medicoId, lista));
	}
}


//public readonly record HorariosMedicos2025Agg(
//	HorarioId Id,
//	MedicoId MedicoId,
//	HorarioFranja2025 Horario
//);
//public readonly record struct HorariosMedicos2025Agg(HorarioId Id, HorarioFranja2025 Horario) {
//	public static HorariosMedicos2025Agg Crear(HorarioId id, HorarioFranja2025 turno) => new(id, turno);
//	public static Result<HorariosMedicos2025Agg> CrearResult(
//		Result<HorarioId> idResult,
//		Result<HorarioFranja2025> pacienteResult
//	)
//		=> from id in idResult
//		   from paciente in pacienteResult
//		   select new HorariosMedicos2025Agg(
//			   id,
//			   paciente
//		   );
//}
/*

public readonly record struct HorariosMedicos2025Agg(
	IReadOnlyList<HorarioFranja2025> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores.Count == 0)
			return "Lista de horarios: (vacía)";
		IEnumerable<string> lineas = Valores
			.Select(v => "- " + v.ATexto());
		return "Lista de horarios:\n" + string.Join("\n", lineas);
	}

	public static Result<HorariosMedicos2025Agg> CrearResult(IEnumerable<HorarioFranja2025>? horariosInput) {
		if (horariosInput is null)
			return new Result<HorariosMedicos2025Agg>.Error("La lista de horarios no puede ser nula.");

        // Materializamos
        List<HorarioFranja2025> lista = [.. horariosInput];

		if (lista.Count == 0)
			return new Result<HorariosMedicos2025Agg>.Error("La lista de horarios no puede estar vacía.");

        // VALIDACION INDIVIDUAL
        List<string> errores = [];

		foreach (HorarioFranja2025 h in lista) {
			if (h.HoraDesde >= h.HoraHasta)
				errores.Add($"El horario {h} tiene un rango horario inválido: Desde debe ser < Hasta.");

			if (h.VigenteDesde > h.VigenteHasta)
				errores.Add($"El horario {h} tiene un rango de vigencia inválido: VigenteDesde debe ser <= VigenteHasta.");
		}

		if (errores.Count > 0)
			return new Result<HorariosMedicos2025Agg>.Error(string.Join("\n", errores));

        // VALIDACION DE SUPERPOSICIONES
        // Agrupar por médico y día
        IEnumerable<IGrouping<(MedicoId MedicoId, DayOfWeek DiaSemana), HorarioFranja2025>> grupos = lista
			.GroupBy(h => (h.MedicoId, h.DiaSemana));

		foreach (IGrouping<(MedicoId MedicoId, DayOfWeek DiaSemana), HorarioFranja2025> g in grupos) {
            List<HorarioFranja2025> horarios = [.. g.OrderBy(h => h.HoraDesde)];

			for (int i = 0; i < horarios.Count - 1; i++) {
                HorarioFranja2025 actual = horarios[i];
                HorarioFranja2025 siguiente = horarios[i + 1];

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
			return new Result<HorariosMedicos2025Agg>.Error(string.Join("\n", errores));

		// Todo ok
		return new Result<HorariosMedicos2025Agg>.Ok(
			new HorariosMedicos2025Agg(lista)
		);
	}




}
*/