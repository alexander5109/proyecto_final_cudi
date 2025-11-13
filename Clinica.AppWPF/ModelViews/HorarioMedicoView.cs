using System.Collections.ObjectModel;

namespace Clinica.AppWPF;

public class HorarioMedicoTimeSpanView {
	public string Desde { get; set; } = string.Empty;
	public string Hasta { get; set; } = string.Empty;
}

public class HorarioMedicoView {
	public string DiaName { get; set; } = string.Empty;
	public ObservableCollection<HorarioMedicoTimeSpanView> FranjasHora { get; set; } = new();
}
