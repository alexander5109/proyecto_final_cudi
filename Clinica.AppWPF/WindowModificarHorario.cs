using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Clinica.AppWPF.Ventanas;

public partial class VMModificarHorario : ObservableObject {
	public ViewModelMedico Medico { get; }

	[ObservableProperty]
	private ViewModelHorario horario;

	public IEnumerable<DayOfWeek> DiaSemanaValues => DiaSemana2025.Los7EnumDias;

	public bool EsNuevo { get; }

	public VMModificarHorario(ViewModelMedico medico, ViewModelHorario horario, bool esNuevo) {
		Medico = medico;
		Horario = horario;
		EsNuevo = esNuevo;
	}
}

public partial class WindowModificarHorario : Window {
	public VMModificarHorario VM { get; }

	public WindowModificarHorario(ViewModelMedico medico, ViewModelHorario horario, bool esNuevo) {
		InitializeComponent();
		VM = new VMModificarHorario(medico, horario, esNuevo);
		DataContext = VM;
	}

	private bool ValidarHorario(ViewModelMedico medico, ViewModelHorario nuevo) {
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
