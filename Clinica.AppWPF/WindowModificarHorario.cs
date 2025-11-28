using Clinica.AppWPF.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Clinica.AppWPF.Ventanas;


public partial class WindowModificarHorario : Window {
	public WindowModificarHorarioViewModel VM { get; }

	public WindowModificarHorario(WindowModificarMedicoViewModel medico, HorarioMedicoViewModel horario, bool esNuevo) {
		InitializeComponent();
		VM = new WindowModificarHorarioViewModel(medico, horario, esNuevo);
		DataContext = VM;
	}

	private bool ValidarHorario(WindowModificarMedicoViewModel medico, HorarioMedicoViewModel nuevo) {
		return !medico.Horarios.Any(h =>
			h != nuevo &&
			h.DiaSemana == nuevo.DiaSemana &&
			!(nuevo.Hasta <= h.Desde || nuevo.Desde >= h.Hasta)
		);
	}

	private void Aceptar_Click(object sender, RoutedEventArgs e) {
		if (!ValidarHorario(VM.Medico, VM.Horario)) {
			MessageBox.Show("El horario colisiona con otro horario del médico.");
			return;
		}

		if (VM.EsNuevo)
			VM.Medico.Horarios.Add(VM.Horario);

		DialogResult = true;
		Close();
	}

	private void Cancelar_Click(object sender, RoutedEventArgs e) {
		Close();
	}
}
