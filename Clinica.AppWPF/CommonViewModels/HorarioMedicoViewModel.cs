using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.ViewModels;

public partial class HorarioMedicoViewModel : ObservableObject {
	[ObservableProperty] public DiaDeSemanaViewModel diaSemana;
	[ObservableProperty] public TimeOnly desde;
	[ObservableProperty] public TimeOnly hasta;
}
