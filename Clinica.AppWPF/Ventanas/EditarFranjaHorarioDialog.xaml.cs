using System.Windows;

namespace Clinica.AppWPF.Ventanas; 
public partial class EditarFranjaHorarioDialog : Window {
	public TimeOnly Desde { get; private set; }
	public TimeOnly Hasta { get; private set; }
	public EditarFranjaHorarioDialog(HorarioMedicoTimeSpanView? franja = null) {
		InitializeComponent();
		if (franja != null) {
			txtDesde.Text = franja.Desde.ToString("HH:mm");
			txtHasta.Text = franja.Hasta.ToString("HH:mm");
		}
	}

	private void Aceptar_Click(object sender, RoutedEventArgs e) {
		if (!TimeOnly.TryParse(txtDesde.Text, out var desde) ||
			!TimeOnly.TryParse(txtHasta.Text, out var hasta)) {
			MessageBox.Show("Formato de hora inválido. Use HH:mm", "Error",
				MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (hasta <= desde) {
			MessageBox.Show("'Hasta' debe ser mayor que 'Desde'.", "Error",
				MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		Desde = desde;
		Hasta = hasta;

		DialogResult = true;
		Close();
	}

	private void Cancelar_Click(object sender, RoutedEventArgs e) {
		DialogResult = false;
		Close();
	}

}
