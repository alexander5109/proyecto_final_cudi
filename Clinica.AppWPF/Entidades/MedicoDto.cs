using Clinica.Dominio.Entidades;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;
using SystemTextJson = System.Text.Json;

namespace Clinica.AppWPF {
	//---------------------------------Tablas.Horarios-------------------------------//


	public class DiaConHorarios {
		public string Nombre { get; set; } = string.Empty;
		public List<HorarioMedico> Horarios { get; set; } = new();
	};

	public class MedicoDto {
		public string ?Id { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }
		public string? Dni { get; set; }
		public string? Provincia { get; set; }
		public string? Domicilio { get; set; }
		public string? Localidad { get; set; }
		public string? Especialidad { get; set; }
		public string? Telefono { get; set; }
		public bool? Guardia { get; set; }
		public DateTime? FechaIngreso { get; set; }
		public double? SueldoMinimoGarantizado { get; set; }
		public ObservableCollection<DiaConHorarios> Agenda { get; } = new();
			
		//usado por comboboxes para mostrar varios campos en un solo place.
		[JsonIgnore]
		public string Displayear => $"{Id}: {Especialidad} - {Name} {LastName}";

	//---------------------------------Constructores-------------------------------//
		public MedicoDto() { }

		// Constructor de mEDICO en base a una ventana
		public MedicoDto(MedicosModificar window){
			LeerDesdeVentana(window);
		}

		// Constructor de MedicoDto para JSON
		public MedicoDto(string jsonElementKey, SystemTextJson.JsonElement jsonElement) {
			Id = jsonElement.GetProperty(nameof(Id)).GetString();
			Dni = jsonElement.GetProperty(nameof(Dni)).GetString();
			Name = jsonElement.GetProperty(nameof(Name)).GetString();
			LastName = jsonElement.GetProperty(nameof(LastName)).GetString();
			Provincia = jsonElement.GetProperty(nameof(Provincia)).GetString();
			Domicilio = jsonElement.GetProperty(nameof(Domicilio)).GetString();
			Localidad = jsonElement.GetProperty(nameof(Localidad)).GetString();
			Especialidad = jsonElement.GetProperty(nameof(Especialidad)).GetString();
			Telefono = jsonElement.GetProperty(nameof(Telefono)).GetString();
			Guardia = jsonElement.GetProperty(nameof(Guardia)).GetBoolean();
			FechaIngreso = DateTime.TryParse(jsonElement.GetProperty(nameof(FechaIngreso)).GetString(), out var fecha) ? fecha : (DateTime?)null;
			SueldoMinimoGarantizado = jsonElement.GetProperty(nameof(SueldoMinimoGarantizado)).GetDouble();

			//if (jsonElement.TryGetProperty(nameof(DiasDeAtencion), out SystemTextJson.JsonElement diasDeAtencionElement)) {
			//	foreach (var dia in diasDeAtencionElement.EnumerateObject()) {
			//		var diaKey = dia.Name;
			//		if (
			//			dia.Value.TryGetProperty("HoraInicio", out var startElement) 
			//			&& dia.Value.TryGetProperty("HoraFin", out var endElement)
			//		) {
			//			DiasDeAtencion[diaKey].HoraInicio = startElement.ToString();
			//			DiasDeAtencion[diaKey].HoraFin = endElement.ToString();
			//		}
			//	}
			//}
		}
		

		//---------------------------------PUBLICOS-------------------------------//
		// Metodo para aplicarle los cambios de una ventana a una instancia de medico existente.
		public void LeerDesdeVentana(MedicosModificar window) {
			this.Name = window.txtName.Text;
			this.LastName = window.txtLastName.Text;
			this.Dni = window.txtDni.Text;
            this.Telefono = window.txtTelefono.Text;
            this.Provincia = window.txtProvincia.Text;
			this.Domicilio = window.txtDomicilio.Text;
			this.Localidad = window.txtLocalidad.Text;
			this.Especialidad = window.txtEspecialidades.SelectedItem.ToString();
			this.FechaIngreso = (DateTime)window.txtFechaIngreso.SelectedDate;
			this.Guardia = (bool)window.txtGuardia.IsChecked;
			this.SueldoMinimoGarantizado = double.Parse(window.txtSueldoMinimoGarantizado.Text);
			//this.DiasDeAtencion = //Al haber pasado los datos como List de HorariosMedicos, los objetos originales fueron modificados in-place. Assi que aca no hay que hacer nada.
			
		}
		
		// Metodo para mostrarse en una ventana
		public void MostrarseEnVentana(MedicosModificar ventana) {
			ventana.txtName.Text = this.Name;
			ventana.txtLastName.Text = this.LastName;
			ventana.txtDni.Text = this.Dni;
            ventana.txtTelefono.Text = this.Telefono;
            ventana.txtProvincia.Text = this.Provincia;
			ventana.txtDomicilio.Text = this.Domicilio;
			ventana.txtLocalidad.Text = this.Localidad;
			ventana.txtEspecialidades.SelectedItem = this.Especialidad;
			ventana.txtFechaIngreso.SelectedDate = this.FechaIngreso;
			ventana.txtGuardia.IsChecked = this.Guardia;
			ventana.txtSueldoMinimoGarantizado.Text = this.SueldoMinimoGarantizado.ToString();
			//ventana.txtDiasDeAtencion.ItemsSource = this.DiasDeAtencion.Values.ToList();
		}
		
		
		
	}
}
