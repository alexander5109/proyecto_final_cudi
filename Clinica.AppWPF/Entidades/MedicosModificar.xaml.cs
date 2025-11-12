using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace Clinica.AppWPF {


	public partial class MedicosModificar : Window {
		private static MedicoDto? SelectedMedico;



		//---------------------public.constructors-------------------//
		public MedicosModificar() {
			InitializeComponent();
			SelectedMedico = null;
			//txtDiasDeAtencion.ItemsSource = HorarioMedico.GetDiasDeLaSemanaAsList();
		}



		// Metodo para mostrarse en una ventana
		public static void MostrarseEnVentana(MedicoDto medicoDto, MedicosModificar ventana) {
			ventana.txtName.Text = medicoDto.Name;
			ventana.txtLastName.Text = medicoDto.LastName;
			ventana.txtDni.Text = medicoDto.Dni;
			ventana.txtTelefono.Text = medicoDto.Telefono;
			ventana.txtProvincia.Text = medicoDto.Provincia;
			ventana.txtDomicilio.Text = medicoDto.Domicilio;
			ventana.txtLocalidad.Text = medicoDto.Localidad;
			ventana.txtFechaIngreso.SelectedDate = medicoDto.FechaIngreso;
			ventana.txtGuardia.IsChecked = medicoDto.Guardia;
			ventana.txtSueldoMinimoGarantizado.Text = medicoDto.SueldoMinimoGarantizado.ToString();
			ventana.txtHorariosMedicos.ItemsSource = medicoDto.Horarios;
			ventana.txtEspecialidades.ItemsSource = App.BaseDeDatos.ReadDistinctEspecialidades();
			ventana.txtEspecialidades.SelectedItem = medicoDto.Especialidad;
			//ventana.txtHorariosMedicos.ItemsSource = ????
			//foreach (var dia in medicoDto.Horarios) {
			//	MessageBox.Show(dia.Nombre);
			//	foreach (var h in dia.Horarios)
			//		MessageBox.Show($"{h.Desde} - {h.Hasta}");
			//}


			//MessageBox.Show(medicoDto.Horarios[0].ToString());


		}


		public MedicosModificar(MedicoDto selectedMedico) {
			InitializeComponent();
			SelectedMedico = selectedMedico;
			MostrarseEnVentana(SelectedMedico, this);


			//txtEspecialidades.SelectedValuePath = "Id";
			//txtEspecialidades.DisplayMemberPath = "Displayear";    //Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
			//Result<Medico2025> resultado = this.ToDomain();

			//if (resultado is Result<Medico2025>.Ok ok) {
			//	var agenda = ok.Value.Horarios;
			//	txtAgendaWidget.ItemsSource = agenda.DisponibilidadEnDia;

			//} else if (resultado is Result<Medico2025>.Error error) {
			//	MessageBox.Show(
			//		$"No se puede cargar la agenda del médico: {error.Mensaje}",
			//		"Error de ingreso",
			//		MessageBoxButton.OK,
			//		MessageBoxImage.Warning
			//	);
			//}
		}





		//public static MedicoDto FromDomain(Medico2025 medico) {
		//	var dias = medico.Horarios
		//		.GroupBy(h => h.HorarioDiaSemana2025)
		//		.Select(g => new DiaConHorarios {
		//			Nombre = g.Key.ToString(), // "Monday", "Tuesday", etc. (puedes traducir si querés)
		//			Horarios = new ObservableCollection<HorarioMedicoView>(
		//				g.Select(h => new HorarioMedicoView {
		//					Desde = h.Desde.ToString("HH:mm"),
		//					Hasta = h.Hasta.ToString("HH:mm")
		//				}))
		//		});
		//	return new MedicoDto {Horarios = new ObservableCollection<DiaConHorarios>(dias)};
		//}






		//---------------------botones.GuardarCambios-------------------//
		private void ButtonGuardar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();

			Result<Medico2025> resultado = this.ToDomain();

			resultado.Switch(
				ok => {
					bool exito;

					if (SelectedMedico is null) {
						SelectedMedico = new MedicoDto(this);
						exito = App.BaseDeDatos.CreateMedico(ok, SelectedMedico);
					} else {
						// Actualizar existente
						SelectedMedico.LeerDesdeVentana(this);
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

		private Result<Medico2025> ToDomain() {
			throw new NotImplementedException();
			Result<NombreCompletoType> nombreResult = NombreCompletoType.Crear(txtName.Text, txtLastName.Text);
			Result<DniArgentinoType> dniResult = DniArgentinoType.Crear(txtDni.Text);
			Result<ContactoTelefonoType> telefonoResult = ContactoTelefonoType.Crear(txtTelefono.Text);
			Result<MedicoEspecialidadType> especialidadResult = MedicoEspecialidadType.Crear(
				txtEspecialidades.SelectedItem.ToString(),
				//MedicoEspecialidad2025.EspecialidadesValidas[int.Parse(txtEspecialidades.SelectedValue.ToString())].Titulo,
				MedicoEspecialidadRamaType.RamasValidas.FirstOrDefault().Titulo
			);

			// Crear la agenda
			//List<HorarioMedico> disponibilidades = new List<HorarioMedico> {
			//	((Result<HorarioMedico>.Ok)lunes).Value,
			//	((Result<HorarioMedico>.Ok)miercoles).Value
			//};

			// Finalmente, la agenda del médico:
			//Result<HorariosMedicos> horariosResult = HorariosMedicos.Crear(
			//	new Result<IReadOnlyList<HorarioMedico>>.Ok(disponibilidades)
			//);



			//Result<IReadOnlyList<HorarioMedico>> horariosResult = Result<>


			//this.txtDiasDeAtencion.ItemsSource;


			Result<ProvinciaDeArgentinaType> provinciaRes = ProvinciaDeArgentinaType.Crear(txtProvincia.Text);
			Result<LocalidadDeProvinciaType> localidadRes = LocalidadDeProvinciaType.Crear(txtLocalidad.Text, provinciaRes);
			Result<DomicilioArgentinoType> domicilioResult = DomicilioArgentinoType.Crear(localidadRes, txtDomicilio.Text);

			Result<FechaIngresoType> fechaIngresoResult = txtFechaIngreso.SelectedDate is DateTime fechaIng
				? FechaIngresoType.Crear(DateOnly.FromDateTime(fechaIng))
				: new Result<FechaIngresoType>.Error("Debe seleccionar una fecha de ingreso válida.");

			Result<MedicoSueldoMinimoType> sueldoResult = MedicoSueldoMinimoType.Crear(txtSueldoMinimoGarantizado.Text);
			bool haceGuardia = txtGuardia.IsChecked is true;


			//return Medico2025.Crear(
			//	nombreResult,
			//	especialidadResult,
			//	dniResult,
			//	domicilioResult,
			//	telefonoResult,
			//	//horariosResult,
			//	fechaIngresoResult,
			//	sueldoResult,
			//	haceGuardia
			//);
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
			//		lista.Add(ok.Value);
			//		txtAgendaWidget.ItemsSource = null;
			//		txtAgendaWidget.ItemsSource = lista.OrderBy(x => x.HorarioDiaSemana2025).ToList();
			//	}
			//}
		}

		private void BtnEditarHorario_Click(object sender, RoutedEventArgs e) {

		}

		private void BtnEliminarHorario_Click(object sender, RoutedEventArgs e) {

		}

		private void txtEspecialidades_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
		}
		//------------------------Fin---------------------------//
	}
}
