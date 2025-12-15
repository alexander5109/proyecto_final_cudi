using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record HorarioDbModel(
		HorarioId Id,
		MedicoId MedicoId,
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
	//	int MedicoId,
	//	IReadOnlyCollection<HorarioDto> Franjas
	//);


	public static HorariosMedicosUpsertDbModel ToUpsertDto(this HorariosMedicos2025Agg agg) {
		var table = new DataTable();
		table.Columns.Add("DiaSemana", typeof(byte));
		table.Columns.Add("HoraDesde", typeof(TimeSpan));
		table.Columns.Add("HoraHasta", typeof(TimeSpan));
		table.Columns.Add("VigenteDesde", typeof(DateOnly));
		table.Columns.Add("VigenteHasta", typeof(DateOnly));

		foreach (var f in agg.Franjas) {
			table.Rows.Add(
				(byte)f.DiaSemana,
				f.HoraDesde.ToTimeSpan(),
				f.HoraHasta.ToTimeSpan(),
				f.VigenteDesde,
				f.VigenteHasta
			);
		}

		return new HorariosMedicosUpsertDbModel(
			agg.MedicoId.Valor,
			table
		);
	}


	public static HorarioDbModel ToModel(this Horario2025Agg aggrg) {
		return new HorarioDbModel(
			Id: aggrg.Id,
			MedicoId: aggrg.Horario.MedicoId,
			DiaSemana: aggrg.Horario.DiaSemana,
			HoraDesde: aggrg.Horario.HoraDesde.ToTimeSpan(),
			HoraHasta: aggrg.Horario.HoraHasta.ToTimeSpan(),
			VigenteDesde: aggrg.Horario.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta: aggrg.Horario.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
		);
	}
	public static HorarioDbModel ToModel(this Horario2025 instance, HorarioId id) {
		return new HorarioDbModel(
			Id: id,
			MedicoId: instance.MedicoId,
			DiaSemana: instance.DiaSemana,
			HoraDesde: instance.HoraDesde.ToTimeSpan(),
			HoraHasta: instance.HoraHasta.ToTimeSpan(),
			VigenteDesde: instance.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta: instance.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
		);
	}

	public static Result<Horario2025Agg> ToDomainAgg(this HorarioDbModel dbModel) {
		return Horario2025Agg.CrearResult(
			HorarioId.CrearResult(dbModel.Id),
			Horario2025.CrearResult(
			dbModel.MedicoId,
			dbModel.DiaSemana,
			TimeOnly.FromTimeSpan(dbModel.HoraDesde),
			TimeOnly.FromTimeSpan(dbModel.HoraHasta),
			new DateOnly(2014, 1, 1),
			new DateOnly(2026, 1, 1)
		));
	}

}