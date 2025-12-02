using System.ComponentModel.DataAnnotations;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static partial class DbModels {
	public class HorarioMedicoModel {
		public HorarioMedicoModel() { }
		public HorarioMedicoModel(int id, int medicoId, DayOfWeek diaSemana, TimeSpan horaDesde, TimeSpan horaHasta, DateTime vigenteDesde, DateTime vigenciaHasta) {
			Id = id;
			MedicoId = medicoId;
			DiaSemana = diaSemana;
			HoraDesde = horaDesde;
			HoraHasta = horaHasta;
			VigenciaDesde = vigenteDesde;
			VigenciaHasta = vigenciaHasta;
		}
		[Key]
		public int Id { get; set; }
		public int MedicoId { get; set; }
		public DayOfWeek DiaSemana { get; set; }
		public TimeSpan HoraDesde { get; set; }
		public TimeSpan HoraHasta { get; set; }
		public DateTime VigenciaDesde { get; set; }
		public DateTime VigenciaHasta { get; set; }
	}
	public static Result<HorarioMedico2025> ToDomain(this HorarioMedicoModel horarioDto ) {
		return HorarioMedico2025.Crear(
			new DiaSemana2025(horarioDto.DiaSemana),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraDesde)),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraHasta)),
			new VigenciaHorario2025(new DateOnly(2014, 1, 1)),
			new VigenciaHorario2025(new DateOnly(2026, 1, 1))
		);
	}

}