using System.ComponentModel.DataAnnotations;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static partial class DbModels {
	public record HorarioMedicoDbModel(
		int Id,
		int MedicoId,
		DayOfWeek DiaSemana,
		TimeSpan HoraDesde,
		TimeSpan HoraHasta,
		DateTime VigenciaDesde,
		DateTime VigenciaHasta
	) {
		public HorarioMedicoDbModel()
			: this(default, default, default, default, default, default, default) { }
	}

	public static Result<HorarioMedico2025> ToDomain(this HorarioMedicoDbModel horarioDto) {
		return HorarioMedico2025.Crear(
			new DiaSemana2025(horarioDto.DiaSemana),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraDesde)),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraHasta)),
			new VigenciaHorario2025(new DateOnly(2014, 1, 1)),
			new VigenciaHorario2025(new DateOnly(2026, 1, 1))
		);
	}

}