namespace Clinica.AppWPF.ViewModels;

public record DiaDeSemanaViewModel(DayOfWeek Value, string NombreDia) {

	public static readonly List<DiaDeSemanaViewModel> Los7DiasDeLaSemana = [
		new(DayOfWeek.Monday,    "Lunes"),
		new(DayOfWeek.Tuesday,   "Martes"),
		new(DayOfWeek.Wednesday, "Miércoles"),
		new(DayOfWeek.Thursday,  "Jueves"),
		new(DayOfWeek.Friday,    "Viernes"),
		new(DayOfWeek.Saturday,  "Sábado"),
		new(DayOfWeek.Sunday,    "Domingo")
	];

};
