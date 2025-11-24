using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

public class HorarioMedicoDto {
	public HorarioMedicoDto() { }
	public HorarioMedicoDto(int id, int medicoId, int diaSemana, TimeSpan horaDesde, TimeSpan horaHasta) {
		Id = id;
		MedicoId = medicoId;
		DiaSemana = diaSemana;
		HoraDesde = horaDesde;
		HoraHasta = horaHasta;
	}
	public int Id { get; set; }
	public int MedicoId { get; set; }
	public int DiaSemana { get; set; }
	public TimeSpan HoraDesde { get; set; }
	public TimeSpan HoraHasta { get; set; }
	public Result<HorarioMedico2025> ToDomain() {
		return HorarioMedico2025.Crear(
			new DiaSemana2025((DayOfWeek)DiaSemana),
			new HorarioHora2025(TimeOnly.FromTimeSpan(HoraDesde)),
			new HorarioHora2025(TimeOnly.FromTimeSpan(HoraHasta))
		);
	}
}
