using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Clinica.AppWPF;

public partial class HorarioMedicoTimeSpanView : ObservableObject {
	[ObservableProperty] private TimeOnly desde;
	[ObservableProperty] private TimeOnly hasta;
}

public class HorarioMedicoView {
	public string DiaName { get; set; } = string.Empty;
	public ObservableCollection<HorarioMedicoTimeSpanView> FranjasHora { get; set; } = new();
}
