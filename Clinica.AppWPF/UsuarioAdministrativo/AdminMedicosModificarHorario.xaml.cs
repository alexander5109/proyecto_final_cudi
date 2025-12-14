using System;
using System.Windows;
using static Clinica.Shared.ApiDtos.MedicoDtos;
using Clinica.AppWPF.UsuarioAdministrativo;

namespace Clinica.AppWPF.Ventanas;


public partial class AdminMedicosModificarHorario : Window {
	private HorarioMedicoViewModel? _vm;

	public AdminMedicosModificarHorario(HorarioMedicoViewModel vm) {
		InitializeComponent();
		_vm = vm;
		// populate fields
		comboDia.ItemsSource = Enum.GetValues(typeof(DayOfWeek));
		comboDia.SelectedItem = vm.DiaSemana;
		txtDesde.Text = vm.HoraDesde.ToString("HH:mm");
		txtHasta.Text = vm.HoraHasta.ToString("HH:mm");
	}

	private void Aceptar_Click(object sender, RoutedEventArgs e) {
		if (_vm is null) { DialogResult = false; return; }
		if (comboDia.SelectedItem is DayOfWeek dia) _vm.DiaSemana = dia;
		if (TimeSpan.TryParse(txtDesde.Text, out var desde)) _vm.HoraDesde = TimeOnly.FromTimeSpan(desde);
		if (TimeSpan.TryParse(txtHasta.Text, out var hasta)) _vm.HoraHasta = TimeOnly.FromTimeSpan(hasta);
		DialogResult = true;
		Close();
	}

	private void Cancelar_Click(object sender, RoutedEventArgs e) {
		DialogResult = false;
		Close();
	}
}
