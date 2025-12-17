using System.Data;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record HorarioDbModel(
		HorarioId2025 Id,
		MedicoId2025 MedicoId,
		DayOfWeek DiaSemana,
		TimeSpan HoraDesde,
		TimeSpan HoraHasta,
		DateTime VigenteDesde,
		DateTime? VigenteHasta
	) {
		public HorarioDbModel()
			: this(default, default, default, default, default, default, default) { }
	}
	public sealed record HorariosMedicosUpsertDbModel(
		int MedicoId,
		DataTable Franjas
	);
	//public sealed record HorariosMedicosUpsertDto(
	//	int MedicoId2025,
	//	IReadOnlyCollection<HorarioDto> Franjas
	//);


	public static HorariosMedicosUpsertDbModel ToUpsertDto(this HorariosMedicos2026Agg agg) {
        DataTable table = new DataTable();
		table.Columns.Add("DiaSemana", typeof(byte));
		table.Columns.Add("HoraDesde", typeof(TimeSpan));
		table.Columns.Add("HoraHasta", typeof(TimeSpan));
		table.Columns.Add("VigenteDesde", typeof(DateTime));
		table.Columns.Add("VigenteHasta", typeof(DateTime));

		foreach (var f in agg.Franjas) {
			table.Rows.Add(
				(byte)f.DiaSemana,
				f.HoraDesde.ToTimeSpan(),
				f.HoraHasta.ToTimeSpan(),
				f.VigenteDesde.ToDateTime(TimeOnly.MinValue),
				f.VigenteHasta?.ToDateTime(TimeOnly.MinValue)
			);
		}

		return new HorariosMedicosUpsertDbModel(
			agg.MedicoId.Valor,
			table
		);
	}


	//public static HorarioDbModel ToModel(this Horario2025Agg aggrg) {
	//	return new HorarioDbModel(
	//		Id: aggrg.Id,
	//		MedicoId2025: aggrg.Horario.MedicoId2025,
	//		DiaSemana: aggrg.Horario.DiaSemana,
	//		HoraDesde: aggrg.Horario.HoraDesde.ToTimeSpan(),
	//		HoraHasta: aggrg.Horario.HoraHasta.ToTimeSpan(),
	//		VigenteDesde: aggrg.Horario.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
	//		VigenteHasta: aggrg.Horario.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
	//	);
	//}
	//public static HorarioDbModel ToModel(this Horario2025 instance, HorarioId2025 id) {
	//	return new HorarioDbModel(
	//		Id: id,
	//		MedicoId2025: instance.MedicoId2025,
	//		DiaSemana: instance.DiaSemana,
	//		HoraDesde: instance.HoraDesde.ToTimeSpan(),
	//		HoraHasta: instance.HoraHasta.ToTimeSpan(),
	//		VigenteDesde: instance.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
	//		VigenteHasta: instance.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
	//	);
	//}

	//public static Result<Horario2025Agg> ToDomainAgg(this HorarioDbModel dbModel) {
	//	return Horario2025Agg.CrearResult(
	//		HorarioId2025.CrearResult(dbModel.Id),
	//		Horario2025.CrearResult(
	//		dbModel.MedicoId2025,
	//		dbModel.DiaSemana,
	//		TimeOnly.FromTimeSpan(dbModel.HoraDesde),
	//		TimeOnly.FromTimeSpan(dbModel.HoraHasta),
	//		new DateOnly(2014, 1, 1),
	//		new DateOnly(2026, 1, 1)
	//	));
	//}

}