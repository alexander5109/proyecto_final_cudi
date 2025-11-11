using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.Windows;

namespace Clinica.AppWPF;
public partial class HorarioEditor : Window {
	public DayOfWeek Dia { get; private set; }
	public TimeOnly Desde { get; private set; }
	public TimeOnly Hasta { get; private set; }
	public bool Confirmado { get; private set; }

	public HorarioEditor(MedicoDisponibilidadEnDia2025? horarioExistente = null) {
		InitializeComponent();

		if (horarioExistente is not null) {
			cmbDiaSemana.SelectedIndex = horarioExistente.Value.DiaSemana.Numero;
			txtDesde.Text = horarioExistente.Value.FranjasHorarias.First().Desde.ToString("HH:mm");
			txtHasta.Text = horarioExistente.Value.FranjasHorarias.First().Hasta.ToString("HH:mm");
		}
	}

	private void BtnGuardar_Click(object sender, RoutedEventArgs e) {
		try {
			Dia = (DayOfWeek)cmbDiaSemana.SelectedIndex;
			Desde = TimeOnly.Parse(txtDesde.Text);
			Hasta = TimeOnly.Parse(txtHasta.Text);
			Confirmado = true;
			Close();
		} catch (Exception ex) {
			MessageBox.Show("Error: " + ex.Message, "Datos inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
		}
	}

	private void BtnCancelar_Click(object sender, RoutedEventArgs e) {
		Confirmado = false;
		Close();
	}
}
