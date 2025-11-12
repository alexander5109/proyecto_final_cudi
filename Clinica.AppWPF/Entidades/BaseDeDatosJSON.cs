using System.Windows;
using System.IO;
using Newtonsoft.Json;
using Clinica.Dominio.Entidades;

namespace Clinica.AppWPF {
	public class BaseDeDatosJSON : BaseDeDatosAbstracta {
		static readonly string medicosPath = "databases/medicos.json";
		static readonly string pacientesPath = "databases/pacientes.json";
		static readonly string turnosPath = "databases/turnos.json";

		public BaseDeDatosJSON() {
			ConectadaExitosamente = (
				JsonCargarMedicosExitosamente(medicosPath)
				&& JsonCargarPacientesExitosamente(pacientesPath)
				&& JsonCargarTurnosExitosamente(turnosPath)
			);
		}


		//------------------------public.CREATE.MedicoDto----------------------//
		public override bool CreateMedico(Medico2025 instancia, MedicoDto instanciaDto) {
			if (DictMedicos.Values.Any(i => i.Dni == instanciaDto.Dni)) {
				MessageBox.Show($"Error de integridad: Ya hay un medico con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			instanciaDto.Id = GenerateNextId(DictMedicos);
			DictMedicos[instanciaDto.Id] = instanciaDto;
			this.JsonUpdateMedicos();
			// MessageBox.Show($"Exito: Se ha creado la instancia de MedicoDto: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		}
		//------------------------public.CREATE.Paciente----------------------//
		public override bool CreatePaciente(PacienteType instancia, Paciente instanciaDto) {
			if (DictPacientes.Values.Any(i => i.Dni == instanciaDto.Dni)) {
				MessageBox.Show($"Error de integridad: Ya hay un paciente con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			instanciaDto.Id = GenerateNextId(DictPacientes);
			DictPacientes[instanciaDto.Id] = instanciaDto;
			this.JsonUpdatePacientes();
			// MessageBox.Show($"Exito: Se ha creado la instancia de Paciente: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		}
		//------------------------public.CREATE.Turno----------------------//
		public override bool CreateTurno(TurnoType instancia, Turno instanciaDto) {
			if (DictTurnos.Values.Any(i => i.PacienteId == instanciaDto.PacienteId && i.MedicoId == instanciaDto.MedicoId && i.Fecha == instanciaDto.Fecha)) {
				MessageBox.Show($"Error de integridad: Ya hay un turno entre ese paciente y ese medico en esa fecha.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (DictTurnos.Values.Any(i => i.MedicoId == instanciaDto.MedicoId && i.Fecha == instanciaDto.Fecha && i.Hora == instanciaDto.Hora)) {
				MessageBox.Show($"Error de integridad: El medico ya tiene un turno ese dia a esa hora.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			instanciaDto.Id = GenerateNextId(DictTurnos);
			DictTurnos[instanciaDto.Id] = instanciaDto;
			this.JsonUpdateTurnos();
			// MessageBox.Show($"Exito: Se ha creado la instancia de Turno con Id: {instancia.Id}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		}








		//------------------------public.READ----------------------//
		public override List<MedicoDto> ReadMedicos() {
			return DictMedicos.Values.Cast<MedicoDto>().ToList();
		}
		public override List<Paciente> ReadPacientes() {
			return DictPacientes.Values.Cast<Paciente>().ToList();
		}
		public override List<Turno> ReadTurnos() {
			return DictTurnos.Values.Cast<Turno>().ToList();
		}










		//------------------------public.UPDATE.MedicoDto----------------------//
		public override bool UpdateMedico(Medico2025 instance, string instanceId) {
			// if (string.IsNullOrEmpty(instancia.Dni)) {
			// MessageBox.Show($"Error: El DNI es un campo obligatorio.", "Faltan datos.", MessageBoxButton.OK, MessageBoxImage.Warning);
			// return false;
			// } 
			if (DictMedicos.Values.Count(i => i.Dni == instance.Dni.Value) > 1) {
				MessageBox.Show($"Error de integridad: Ya hay un medico con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			this.JsonUpdateMedicos(); // Guardar los cambios en el archivo JSON
									  // MessageBox.Show($"Exito: Se han actualizado los datos de: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		}
		//------------------------public.UPDATE.Paciente----------------------//
		public override bool UpdatePaciente(PacienteType instancia, string instanceId) {
			// if (string.IsNullOrEmpty(instancia.Dni)) {
			// MessageBox.Show($"Error: El DNI es un campo obligatorio.", "Faltan datos.", MessageBoxButton.OK, MessageBoxImage.Warning);
			// return false;
			// } 
			if (DictPacientes.Values.Count(i => i.Dni == instancia.Dni.Value) > 1) {
				MessageBox.Show($"Error de integridad: Ya hay un paciente con ese Dni. \n No se guardarán los cambios.", "Error de integridad", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			this.JsonUpdatePacientes(); // Guardar los cambios en el archivo JSON
										// MessageBox.Show($"Exito: Se han actualizado los datos de: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		}
		//------------------------public.UPDATE.Turno----------------------//
		public override bool UpdateTurno(TurnoType instanciaValidada, Turno instancia) {
			if (DictTurnos.Values.Count(i => i.PacienteId == instancia.PacienteId && i.MedicoId == instancia.MedicoId && i.Fecha == instancia.Fecha) > 1) {
				MessageBox.Show($"Error de integridad: Ya hay un turno entre ese paciente y ese medico en esa fecha.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (DictTurnos.Values.Count(i => i.MedicoId == instancia.MedicoId && i.Fecha == instancia.Fecha && i.Hora == instancia.Hora) > 1) {
				MessageBox.Show($"Error de integridad: El medico ya tiene un turno ese dia a esa hora.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			this.JsonUpdateTurnos(); // Guardar los cambios en el archivo JSON
									 // MessageBox.Show($"Exito: Se han actualizado los datos del turno Id: {instancia.Id}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		}











		//------------------------public.DELETE.MedicoDto----------------------//
		public override bool DeleteMedico(MedicoDto instancia) {
			if (DictTurnos.Values.Any(i => i.MedicoId == instancia.Id)) {
				MessageBox.Show($"Error de integridad: El medico tiene turnos asignados.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			try {
				DictMedicos.Remove(instancia.Id);
				this.JsonUpdateMedicos(); // Save changes to the database
										  // MessageBox.Show($"Exito: Se ha eliminado el medico con id: {instancia.Id} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
				return true;
			} catch (Exception ex) {
				MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el medico", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;

			}
		}
		//------------------------public.DELETE.Paciente----------------------//
		public override bool DeletePaciente(Paciente instancia) {
			if (DictTurnos.Values.Any(i => i.PacienteId == instancia.Id)) {
				MessageBox.Show($"Error de integridad: El paciente tiene turnos asignados.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			try {
				DictPacientes.Remove(instancia.Id);
				this.JsonUpdatePacientes(); // Save changes to the database
											// MessageBox.Show($"Exito: Se ha eliminado el paciente con id: {instancia.Id} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
				return true;
			} catch (Exception ex) {
				MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el paciente", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}
		//------------------------public.DELETE.Turno----------------------//
		public override bool DeleteTurno(Turno instancia) {
			try {
				DictTurnos.Remove(instancia.Id);
				this.JsonUpdateTurnos(); // Save changes to the database
										 // MessageBox.Show($"Exito: Se ha eliminado el turno con id: {instancia.Id} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
				return true;
			} catch (Exception ex) {
				MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el turno", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}








		//------------------------private.LOAD.Medicos----------------------//
		private bool JsonCargarMedicosExitosamente(string file_path) {
			if (File.Exists(file_path)) {
				try {
					string jsonString = File.ReadAllText(file_path);
					var rawMedicosData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
					var medicos = new Dictionary<string, MedicoDto>();

					foreach (var medicoEntry in rawMedicosData) {
						var medicoJsonElement = System.Text.Json.JsonDocument.Parse(medicoEntry.Value.ToString()).RootElement;
						var medicoInstance = new MedicoDto(medicoEntry.Key, medicoJsonElement);
						medicos[medicoEntry.Key] = medicoInstance;
					}
					DictMedicos = medicos;
				} catch (JsonException ex) {
					MessageBox.Show($"Error parseando el archivo Json {file_path}: {ex.Message}", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
			} else {
				MessageBox.Show($"Error: {file_path} no se encontró.", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			return true;
		}
		//------------------------private.LOAD.Pacientes----------------------//
		private bool JsonCargarPacientesExitosamente(string file_path) {
			if (File.Exists(pacientesPath)) {
				try {
					string jsonString = File.ReadAllText(pacientesPath);
					DictPacientes = JsonConvert.DeserializeObject<Dictionary<string, Paciente>>(jsonString);
				} catch (JsonException ex) {
					MessageBox.Show($"Error parseando el archivo Json {file_path}: {ex.Message}", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
			} else {
				MessageBox.Show($"Error: {file_path} no se encontró.", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			return true;
		}
		//------------------------private.LOAD.Turnos----------------------//
		private bool JsonCargarTurnosExitosamente(string file_path) {
			if (File.Exists(turnosPath)) {
				try {
					string jsonString = File.ReadAllText(turnosPath);
					DictTurnos = JsonConvert.DeserializeObject<Dictionary<string, Turno>>(jsonString);
				} catch (JsonException ex) {
					MessageBox.Show($"Error parseando el archivo Json {file_path}: {ex.Message}", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
			} else {
				MessageBox.Show($"Error: {file_path} no se encontró.", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			return true;
		}



		//------------------------private.SAVE---------------------------//
		private void JsonUpdatePacientes() {
			File.WriteAllText(pacientesPath, JsonConvert.SerializeObject(DictPacientes, Formatting.Indented));
		}
		private void JsonUpdateMedicos() {
			File.WriteAllText(medicosPath, JsonConvert.SerializeObject(DictMedicos, Formatting.Indented));
		}
		private void JsonUpdateTurnos() {
			File.WriteAllText(turnosPath, JsonConvert.SerializeObject(DictTurnos, Formatting.Indented));
		}
		//------------------------private.CONSTRAINTS----------------------//
		private string GenerateNextId<T>(Dictionary<string, T> dictionary) {
			int maxId = 0;

			foreach (var key in dictionary.Keys) {
				if (int.TryParse(key, out int numericId)) {
					maxId = Math.Max(maxId, numericId);
				}
			}
			return (maxId + 1).ToString();
		}
		//------------------------settings----------------------//
		// settings
		//public override bool EliminarDatabaseExitosamente() {
		//return false;
		//}
		//------------------------Fin.BaseDeDatosJSON----------------------//
	}
}
