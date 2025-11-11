using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using System.Windows;

namespace Clinica.AppWPF {
	public partial class MedicosModificar : Window {
		private static Medico? SelectedMedico;
		//---------------------public.constructors-------------------//
		public MedicosModificar() //Constructor con un objeto como parametro ==> Modificarlo.
		{
			InitializeComponent();
			SelectedMedico = null;
			//txtDiasDeAtencion.ItemsSource = HorarioMedico.GetDiasDeLaSemanaAsList();
		}

		public MedicosModificar(Medico selectedMedico) {
			InitializeComponent();
			SelectedMedico = selectedMedico;
			SelectedMedico.MostrarseEnVentana(this);


			//txtEspecialidades.SelectedValuePath = "Id";
			//txtEspecialidades.DisplayMemberPath = "Displayear";    //Property de cada Objeto para mostrarse como una union de dni nombre y apellido. 
			txtEspecialidades.ItemsSource = App.BaseDeDatos.ReadDistinctEspecialidades();
			txtEspecialidades.SelectedItem = SelectedMedico.Especialidad;
			//Result<Medico2025> resultado = this.ToDomain();

			//if (resultado is Result<Medico2025>.Ok ok) {
			//	var agenda = ok.Value.Agenda;
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



		//--------------------AsegurarInput-------------------//
		/*
		private bool CamposCompletadosCorrectamente() {
			if (
				this.txtSueldoMinimoGarantizado.Text is null
				|| this.txtDni.Text is null
				|| this.txtFechaIngreso.SelectedDate is null
				|| this.txtGuardia.IsChecked is null
			) {
				MessageBox.Show($"Error: Faltan datos obligatorios por completar.", "Error de ingreso.", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			}

			if (!Int64.TryParse(this.txtDni.Text, out _)) {
				MessageBox.Show($"Error: El dni no es un numero entero valido.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			}

			if (!Double.TryParse(this.txtSueldoMinimoGarantizado.Text, out _)) {
				MessageBox.Show("Error: El sueldo minimo no es un número decimal válido. Use la coma (,) como separador decimal.", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			}
			foreach (HorarioMedico campo in this.txtDiasDeAtencion.ItemsSource as List<HorarioMedico>) {
				if (string.IsNullOrEmpty(campo.HoraInicio) && string.IsNullOrEmpty(campo.HoraFin)) {
					continue;
				}
				if (App.TryParseHoraField(campo.HoraInicio) && App.TryParseHoraField(campo.HoraFin)) {
					continue;
				} else {
					MessageBox.Show($"Error: No se reconoce el horario del día {campo.DiaSemana}. \nIngrese un string con formato valido (hh:mm)", "Error de ingreso", MessageBoxButton.OK, MessageBoxImage.Warning);
					return false;
				}
			}

			return true;
		}
		*/


		//---------------------botones.GuardarCambios-------------------//
		private void ButtonGuardar(object sender, RoutedEventArgs e) {
			App.PlayClickJewel();

			Result<Medico2025> resultado = this.ToDomain();

			resultado.Switch(
				ok => {
					bool exito;

					if (SelectedMedico is null) {
						SelectedMedico = new Medico(this);
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
			Result<NombreCompleto2025> nombreResult = NombreCompleto2025.Crear(txtName.Text, txtLastName.Text);
			Result<DniArgentino2025> dniResult = DniArgentino2025.Crear(txtDni.Text);
			Result<Contacto2025Telefono> telefonoResult = Contacto2025Telefono.Crear(txtTelefono.Text);
			Result<MedicoEspecialidad2025> especialidadResult = MedicoEspecialidad2025.Crear(
				txtEspecialidades.SelectedItem.ToString(),
				//MedicoEspecialidad2025.EspecialidadesValidas[int.Parse(txtEspecialidades.SelectedValue.ToString())].Titulo,
				MedicoEspecialidadRama.RamasValidas.FirstOrDefault().Titulo
			);

			// Crear franjas horarias
			Result<MedicoFranjaHoraria2025> franjaManiana = MedicoFranjaHoraria2025.Crear(new TimeOnly(8, 0), new TimeOnly(12, 0));
			Result<MedicoFranjaHoraria2025> franjaTarde = MedicoFranjaHoraria2025.Crear(new TimeOnly(14, 0), new TimeOnly(18, 0));
			Result<MedicoFranjaHoraria2025> franjaMiercoles = MedicoFranjaHoraria2025.Crear(new TimeOnly(10, 0), new TimeOnly(16, 0));

			// Crear disponibilidades por día
			Result<MedicoDisponibilidadEnDia2025> lunes = MedicoDisponibilidadEnDia2025.Crear(
				new MedicoDiaDeLaSemana2025(DayOfWeek.Monday),
				new[] { ((Result<MedicoFranjaHoraria2025>.Ok)franjaManiana).Value, ((Result<MedicoFranjaHoraria2025>.Ok)franjaTarde).Value }
			);

			Result<MedicoDisponibilidadEnDia2025> miercoles = MedicoDisponibilidadEnDia2025.Crear(
				new MedicoDiaDeLaSemana2025(DayOfWeek.Wednesday),
				new[] { ((Result<MedicoFranjaHoraria2025>.Ok)franjaMiercoles).Value }
			);

			// Crear la agenda
			List<MedicoDisponibilidadEnDia2025> disponibilidades = new List<MedicoDisponibilidadEnDia2025> {
				((Result<MedicoDisponibilidadEnDia2025>.Ok)lunes).Value,
				((Result<MedicoDisponibilidadEnDia2025>.Ok)miercoles).Value
			};

			// Finalmente, la agenda del médico:
			Result<MedicoAgenda2025> agendaResult = MedicoAgenda2025.Crear(
				new Result<IReadOnlyList<MedicoDisponibilidadEnDia2025>>.Ok(disponibilidades)
			);

			//this.txtDiasDeAtencion.ItemsSource;


			Result<ProvinciaDeArgentina2025> provinciaRes = ProvinciaDeArgentina2025.Crear(txtProvincia.Text);
			Result<LocalidadDeProvincia2025> localidadRes = LocalidadDeProvincia2025.Crear(txtLocalidad.Text, provinciaRes);
			Result<DomicilioArgentino2025> domicilioResult = DomicilioArgentino2025.Crear(localidadRes, txtDomicilio.Text);

			Result<FechaIngreso2025> fechaIngresoResult = txtFechaIngreso.SelectedDate is DateTime fechaIng
				? FechaIngreso2025.Crear(DateOnly.FromDateTime(fechaIng))
				: new Result<FechaIngreso2025>.Error("Debe seleccionar una fecha de ingreso válida.");

			Result<MedicoSueldoMinimoGarantizado2025> sueldoResult = MedicoSueldoMinimoGarantizado2025.Crear(txtSueldoMinimoGarantizado.Text);
			bool haceGuardia = txtGuardia.IsChecked is true;


			return Medico2025.Crear(
				nombreResult,
				especialidadResult,
				dniResult,
				domicilioResult,
				telefonoResult,
				agendaResult,
				fechaIngresoResult,
				sueldoResult,
				haceGuardia
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
			HorarioEditor editor = new HorarioEditor();
			editor.Owner = this;
			if (editor.ShowDialog() == true && editor.Confirmado) {
				Result<MedicoDisponibilidadEnDia2025> resultado = MedicoDisponibilidadEnDia2025.Crear(
					new MedicoDiaDeLaSemana2025(editor.Dia),
					new[] {
				new MedicoFranjaHoraria2025(editor.Desde, editor.Hasta)
					}
				);

				if (resultado is Result<MedicoDisponibilidadEnDia2025>.Ok ok) {
					List<MedicoDisponibilidadEnDia2025> lista = (List<MedicoDisponibilidadEnDia2025>)txtAgendaWidget.ItemsSource ?? new();
					lista.Add(ok.Value);
					txtAgendaWidget.ItemsSource = null;
					txtAgendaWidget.ItemsSource = lista.OrderBy(x => x.DiaSemana).ToList();
				}
			}
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
