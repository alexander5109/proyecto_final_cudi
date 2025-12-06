using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.ViewModels;

public partial class ViewModelHorariosAgrupados(DiaDeSemanaViewModel dia) : ObservableObject {
	public DiaDeSemanaViewModel DiaSemana { get; } = dia;

	[ObservableProperty] private ObservableCollection<HorarioMedicoViewModel> horarios = [];
}
