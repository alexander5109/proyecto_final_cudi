
using Clinica.AppWPF.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.Ventanas;

public partial class WindowModificarHorarioViewModel : ObservableObject {
	public WindowModificarMedicoViewModel Medico { get; }

	[ObservableProperty]
	private HorarioMedicoViewModel horario;

	//public IEnumerable<DayOfWeek> DiaSemanaValues => new WindowModificarHorarioViewModel();

	public bool EsNuevo { get; }

	public WindowModificarHorarioViewModel(WindowModificarMedicoViewModel medico, HorarioMedicoViewModel horario, bool esNuevo) {
		Medico = medico;
		Horario = horario;
		EsNuevo = esNuevo;
	}
}