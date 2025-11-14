using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Tipos;
using System.Windows;

namespace Clinica.AppWPF.Ventanas;
public partial class WindowModificarHorario : Window {

	public DayOfWeek[] DiaSemanaValues => Enum.GetValues<DayOfWeek>();
	public ModelViewHorario SelectedHorario { get; }
	public ModelViewMedico SelectedMedico { get;}


	public WindowModificarHorario(ModelViewMedico medico, ModelViewHorario horario) {
		InitializeComponent();
		SelectedMedico = medico;
		SelectedHorario = horario;
		DataContext = this;
	}


	//public string[] Los7DiasDeLaSemana = DiaSemana2025.Los7DiasDeLaSemana;
	//public TimeOnly Desde { get; private set; }
	//public TimeOnly Hasta { get; private set; }
	//public WindowModificarHorario(HorarioMedicoTimeSpanView? horario = null) {
	//	InitializeComponent();
	//	if (horario != null) {
	//		txtDesde.Text = horario.Desde.ToString("HH:mm");
	//		txtHasta.Text = horario.Hasta.ToString("HH:mm");
	//	}
	//}

	private void Aceptar_Click(object sender, RoutedEventArgs e) {
		var horarios = SelectedMedico.Horarios
			.Where(h => h != SelectedHorario && h.DiaSemana == SelectedHorario.DiaSemana);

		foreach (var h in horarios) {
			if (!(SelectedHorario.Hasta <= h.Desde || SelectedHorario.Desde >= h.Hasta)) {
				MessageBox.Show("El horario choca con otra horario existente.");
				return;
			}
		}

		DialogResult = true;
	}


	private void Cancelar_Click(object sender, RoutedEventArgs e) {
		DialogResult = false;
		Close();
	}
}
