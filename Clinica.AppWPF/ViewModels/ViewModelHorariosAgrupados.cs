using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.ViewModels;

public partial class ViewModelHorariosAgrupados : ObservableObject {
	public DiaDeSemanaViewModel DiaSemana { get; }

	[ObservableProperty] private ObservableCollection<HorarioMedicoViewModel> horarios = [];

	public ViewModelHorariosAgrupados(DiaDeSemanaViewModel dia) {
		DiaSemana = dia;
	}
}
