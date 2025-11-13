using Clinica.AppWPF.Entidades;
using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace Clinica.AppWPF;


public partial class MedicosModificar : Window, INotifyPropertyChanged{
	private MedicoView _selectedMedico;

	public MedicoView SelectedMedico {
		get => _selectedMedico;
		set {
			_selectedMedico = value;
			OnPropertyChanged(nameof(SelectedMedico));
		}
	}
	//---------------------public.constructors-------------------//
	public MedicosModificar() {
		InitializeComponent();
		DataContext = this;
		SelectedMedico = MedicoView.NewEmpty(); // instancia vacía lista para bindear
	}

	// Constructor para editar un médico existente
	public MedicosModificar(MedicoView selectedMedico) {
		InitializeComponent();
		DataContext = this;
		SelectedMedico = selectedMedico;
	}





	//public static MedicoView FromDomain(Medico2025 medico) {
	//	var dias = medico.Horarios
	//		.GroupBy(h => h.DiaSemana2025)
	//		.Select(g => new HorarioMedicoView {
	//			Nombre = g.Key.ToString(), // "Monday", "Tuesday", etc. (puedes traducir si querés)
	//			Horarios = new ObservableCollection<HorarioMedicoTimeSpanView>(
	//				g.Select(h => new HorarioMedicoTimeSpanView {
	//					Desde = h.Desde.ToString("HH:mm"),
	//					Hasta = h.Hasta.ToString("HH:mm")
	//				}))
	//		});
	//	return new MedicoView {Horarios = new ObservableCollection<HorarioMedicoView>(dias)};
	//}






	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();

		if (SelectedMedico is null) {
			MessageBox.Show("No hay médico seleccionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		Result<Medico2025> resultado = SelectedMedico.ToDomain();

		resultado.Switch(
			ok => {
				bool exito;

				if (SelectedMedico.Id is null) {
					// Crear nuevo médico
					exito = App.BaseDeDatos.CreateMedico(ok, SelectedMedico);
				} else {
					// Actualizar médico existente
					exito = App.BaseDeDatos.UpdateMedico(ok, SelectedMedico.Id);
				}

				if (exito)
					this.Cerrar();
			},
			error => {
				MessageBox.Show(
					$"No se puede guardar el médico: {error}",
					"Error de ingreso",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);
			}
		);
	}




	//---------------------botones.Eliminar-------------------//
	private void ButtonEliminar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();
		//---------Checknulls-----------//
		if (SelectedMedico is null || SelectedMedico.Dni is null) {
			MessageBox.Show($"No hay item seleccionado.");
			return;
		}
		//---------confirmacion-----------//
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedMedico.Name}", "Confirmar Eliminación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) {
			return;
		}
		//---------Eliminar-----------//
		if (App.BaseDeDatos.DeleteMedico(SelectedMedico)) {
			this.Cerrar(); // this.NavegarA<Medicos>();
		}
	}



	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<Medicos>();
	}

	private void ButtonSalir(object sender, RoutedEventArgs e) {
		this.Salir();
	}



	//------------------botonesParaCrear------------------//
	private void ButtonAgregarMedico(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<MedicosModificar>();
	}
	private void ButtonAgregarPaciente(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<PacientesModificar>(); // this.NavegarA<PacientesModificar>();
	}
	private void ButtonAgregarTurno(object sender, RoutedEventArgs e) {
		this.AbrirComoDialogo<MedicosModificar>();
	}

	private void BtnAgregarHorario_Click(object sender, RoutedEventArgs e) {
		//HorarioEditor editor = new HorarioEditor();
		//editor.Owner = this;
		//if (editor.ShowDialog() == true && editor.Confirmado) {
		//	Result<HorarioMedico> resultado = HorarioMedico.Crear(
		//		new MedicoDiaDeLaSemana2025(editor.Dia),
		//		new[] {
		//	new MedicoFranjaHoraria2025(editor.Desde, editor.Hasta)
		//		}
		//	);

		//	if (resultado is Result<HorarioMedico>.Ok ok) {
		//		List<HorarioMedico> lista = (List<HorarioMedico>)txtAgendaWidget.ItemsSource ?? new();
		//		lista.Add(ok.Valor);
		//		txtAgendaWidget.ItemsSource = null;
		//		txtAgendaWidget.ItemsSource = lista.OrderBy(x => x.DiaSemana2025).ToList();
		//	}
		//}
	}



	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string propertyName)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	private void BtnEditarHorario_Click(object sender, RoutedEventArgs e) {

	}

	private void BtnEliminarHorario_Click(object sender, RoutedEventArgs e) {

	}

	private void txtEspecialidades_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
	}
	//------------------------Fin---------------------------//
}
