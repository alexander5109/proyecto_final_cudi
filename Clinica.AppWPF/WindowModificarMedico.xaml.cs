using Clinica.AppWPF.ModelViews;
using Clinica.AppWPF.Ventanas;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Clinica.AppWPF;

public partial class WindowModificarMedico : Window, INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


	//private ObservableCollection<ModelViewHorariosAgrupados> _horariosSemanaCompleta = [];
	//public ObservableCollection<ModelViewHorariosAgrupados> HorariosSemanaCompleta {
	//	get => _horariosSemanaCompleta;
	//	set {
	//		_horariosSemanaCompleta = value;
	//		OnPropertyChanged(nameof(HorariosSemanaCompleta));
	//	}
	//}


	public ModelViewMedico _selectedMedico = ModelViewMedico.NewEmpty();
	public ModelViewMedico SelectedMedico { get => _selectedMedico; set { _selectedMedico = value; OnPropertyChanged(nameof(SelectedMedico)); } }

	public WindowModificarMedico() {
		InitializeComponent();
		DataContext = this;
	}

	public WindowModificarMedico(ModelViewMedico selectedMedico) {
		InitializeComponent();
		SelectedMedico = selectedMedico;
		DataContext = this;
		//var horarios = SelectedMedico.Horarios
		//	.Select(h => $"DiaEnum{h.DiaSemana} {h.DiaSemana.AEspañol()} {h.Desde:hh\\:mm}-{h.Hasta:hh\\:mm}")
		//	.ToList();

		//MessageBox.Show(
		//	$"Médico: {SelectedMedico.LastName}, {SelectedMedico.Name}\n" +
		//	$"Especialidad: {SelectedMedico.Especialidad}\n\n" +
		//	$"Horarios:\n{string.Join("\n", horarios)}",
		//	"Debug Medico",
		//	MessageBoxButton.OK,
		//	MessageBoxImage.Information
		//);
	}
	//---------------------botones.GuardarCambios-------------------//
	private void ButtonGuardar(object sender, RoutedEventArgs e) {
		App.PlayClickJewel();
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
		if (MessageBox.Show($"¿Está seguro que desea eliminar este médico? {SelectedMedico.Name}", "Confirmar Eliminación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) {
			return;
		}
		if (App.BaseDeDatos.DeleteMedico(SelectedMedico)) {
			this.Cerrar(); // this.NavegarA<WindowListarMedicos>();
		}
	}

	private object? GetSelectedTreeItem() {
		return (treeViewHorarios.SelectedItem ??
				(this.FindName("treeViewHorarios") as TreeView)?.SelectedItem);
	}

	//---------------------botones.Salida-------------------//
	private void ButtonCancelar(object sender, RoutedEventArgs e) {
		this.Cerrar(); // this.NavegarA<WindowListarMedicos>();
	}


	private void BtnEditarHorario_Click(object sender, RoutedEventArgs e) { 
	}
	private void BtnEliminarHorario_Click(object sender, RoutedEventArgs e) {
		var selected = GetSelectedTreeItem();

		//if (selected is not ModelViewHorariosAgrupados franja) {
		//	MessageBox.Show("Seleccioná una franja para eliminar.",
		//		"Info", MessageBoxButton.OK, MessageBoxImage.Information);
		//	return;
		//}

		//if (MessageBox.Show("¿Eliminar esta franja?", "Confirmar",
		//	MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
		//	return;

		//foreach (var dia in SelectedMedico.Horarios) {
		//	if (dia.FranjasHora.Contains(franja)) {
		//		dia.FranjasHora.Remove(franja);
		//		break;
		//	}
		//}
	}

	private void BtnAgregarHorarioFranja_Click(object sender, RoutedEventArgs e) {
		//var selected = GetSelectedTreeItem();

		//if (selected is not ModelViewHorario dia) {
		//	MessageBox.Show("Seleccioná un día para agregar una franja.",
		//		"Info", MessageBoxButton.OK, MessageBoxImage.Information);
		//	return;
		//}

		//var dialog = new WindowModificarHorario();
		//dialog.Owner = this;

		//if (dialog.ShowDialog() == true) {
		//	dia.FranjasHora.Add(new HorarioMedicoTimeSpanView {
		//		Desde = dialog.Desde,
		//		Hasta = dialog.Hasta
		//	});
		//}

	}

	//------------------------Fin---------------------------//

}
