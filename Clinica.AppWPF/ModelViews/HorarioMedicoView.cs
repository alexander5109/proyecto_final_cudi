using System.Collections.ObjectModel;

namespace Clinica.AppWPF;

public class HorarioMedicoTimeSpanView {
	public string Desde { get; set; } = string.Empty;
	public string Hasta { get; set; } = string.Empty;
}

public class HorarioMedicoView {
	public string Nombre { get; set; } = string.Empty;
	public ObservableCollection<HorarioMedicoTimeSpanView> Horarios { get; set; } = new();
}
