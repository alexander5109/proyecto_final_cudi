using System.Windows;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Ventanas;


public partial class WindowModificarHorario : Window {
	public HorarioModificarViewModel VM { get; }

	public WindowModificarHorario(MedicoDto medico, HorarioMedicoDto horario, bool esNuevo) {
		InitializeComponent();
		VM = new HorarioModificarViewModel(medico, horario, esNuevo);
		DataContext = VM;
	}

	private bool ValidarHorario(MedicoDto medico, HorarioMedicoDto nuevo) {
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
