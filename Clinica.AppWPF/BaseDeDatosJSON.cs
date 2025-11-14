using Clinica.AppWPF.Entidades;
using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Entidades;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Clinica.AppWPF;
public class BaseDeDatosJSON : BaseDeDatosAbstracta {
	static readonly string medicosPath = "databases/medicos.json";
	static readonly string pacientesPath = "databases/pacientes.json";
	static readonly string turnosPath = "databases/turnos.json";

	public BaseDeDatosJSON() {
		ConectadaExitosamente =
			JsonCargarMedicosExitosamente(medicosPath)
			&& JsonCargarPacientesExitosamente(pacientesPath)
			&& JsonCargarTurnosExitosamente(turnosPath)
		;
	}


	//------------------------public.CREATE.MedicoView----------------------//
	public override bool CreateMedico(Medico2025 instancia, MedicoView instanciaDto) {
		if (DictMedicos.Values.Any(i => i.Dni == instanciaDto.Dni)) {
			MessageBox.Show($"Error de integridad: Ya hay un medico con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		instanciaDto.Id = GenerateNextId(DictMedicos);
		DictMedicos[instanciaDto.Id] = instanciaDto;
		JsonUpdateMedicos();
		// MessageBox.Show($"Exito: Se ha creado la instancia de MedicoView: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}
	//------------------------public.CREATE.PacienteView----------------------//
	public override bool CreatePaciente(Paciente2025 instancia, PacienteView instanciaDto) {
		if (DictPacientes.Values.Any(i => i.Dni == instanciaDto.Dni)) {
			MessageBox.Show($"Error de integridad: Ya hay un paciente con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		instanciaDto.Id = GenerateNextId(DictPacientes);
		DictPacientes[instanciaDto.Id] = instanciaDto;
		JsonUpdatePacientes();
		// MessageBox.Show($"Exito: Se ha creado la instancia de PacienteView: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}
	//------------------------public.CREATE.TurnoView----------------------//
	public override bool CreateTurno(Turno2025 instancia, TurnoView instanciaDto) {
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
		JsonUpdateTurnos();
		// MessageBox.Show($"Exito: Se ha creado la instancia de TurnoView con Id: {instancia.Id}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}



	//public PacienteView(SystemTextJson.JsonElement json) {
	//	Id = json.GetProperty(nameof(Id)).GetString();
	//	Dni = json.GetProperty(nameof(Dni)).GetString();
	//	Name = json.GetProperty(nameof(Name)).GetString();
	//	LastName = json.GetProperty(nameof(LastName)).GetString();
	//	FechaIngreso = json.GetProperty(nameof(FechaIngreso)).GetDateTime();
	//	Email = json.GetProperty(nameof(Email)).GetString();
	//	Telefono = json.GetProperty(nameof(Telefono)).GetString();
	//	FechaNacimiento = json.GetProperty(nameof(FechaNacimiento)).GetDateTime();
	//	Domicilio = json.GetProperty(nameof(Domicilio)).GetString();
	//	Localidad = json.GetProperty(nameof(Localidad)).GetString();
	//	Provincia = json.GetProperty(nameof(Provincia)).GetString();
	//}





	//------------------------public.READ----------------------//
	public override List<MedicoView> ReadMedicos() {
		return DictMedicos.Values.Cast<MedicoView>().ToList();
	}
	public override List<PacienteView> ReadPacientes() {
		return DictPacientes.Values.Cast<PacienteView>().ToList();
	}
	public override List<TurnoView> ReadTurnos() {
		return DictTurnos.Values.Cast<TurnoView>().ToList();
	}










	//------------------------public.UPDATE.MedicoView----------------------//
	public override bool UpdateMedico(Medico2025 instance, string instanceId) {
		// if (string.IsNullOrEmpty(instancia.Dni)) {
		// MessageBox.Show($"Error: El DNI es un campo obligatorio.", "Faltan datos.", MessageBoxButton.OK, MessageBoxImage.Warning);
		// return false;
		// } 
		if (DictMedicos.Values.Count(i => i.Dni == instance.Dni.Valor) > 1) {
			MessageBox.Show($"Error de integridad: Ya hay un medico con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		JsonUpdateMedicos(); // Guardar los cambios en el archivo JSON
							 // MessageBox.Show($"Exito: Se han actualizado los datos de: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}
	//------------------------public.UPDATE.PacienteView----------------------//
	public override bool UpdatePaciente(Paciente2025 instancia, string instanceId) {
		// if (string.IsNullOrEmpty(instancia.Dni)) {
		// MessageBox.Show($"Error: El DNI es un campo obligatorio.", "Faltan datos.", MessageBoxButton.OK, MessageBoxImage.Warning);
		// return false;
		// } 
		if (DictPacientes.Values.Count(i => i.Dni == instancia.Dni.Valor) > 1) {
			MessageBox.Show($"Error de integridad: Ya hay un paciente con ese Dni. \n No se guardarán los cambios.", "Error de integridad", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		JsonUpdatePacientes(); // Guardar los cambios en el archivo JSON
							   // MessageBox.Show($"Exito: Se han actualizado los datos de: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}
	//------------------------public.UPDATE.TurnoView----------------------//
	public override bool UpdateTurno(Turno2025 instanciaValidada, TurnoView instancia) {
		if (DictTurnos.Values.Count(i => i.PacienteId == instancia.PacienteId && i.MedicoId == instancia.MedicoId && i.Fecha == instancia.Fecha) > 1) {
			MessageBox.Show($"Error de integridad: Ya hay un turno entre ese paciente y ese medico en esa fecha.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (DictTurnos.Values.Count(i => i.MedicoId == instancia.MedicoId && i.Fecha == instancia.Fecha && i.Hora == instancia.Hora) > 1) {
			MessageBox.Show($"Error de integridad: El medico ya tiene un turno ese dia a esa hora.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		JsonUpdateTurnos(); // Guardar los cambios en el archivo JSON
							// MessageBox.Show($"Exito: Se han actualizado los datos del turno Id: {instancia.Id}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}











	//------------------------public.DELETE.MedicoView----------------------//
	public override bool DeleteMedico(MedicoView instancia) {
		if (DictTurnos.Values.Any(i => i.MedicoId == instancia.Id)) {
			MessageBox.Show($"Error de integridad: El medico tiene turnos asignados.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		try {
			DictMedicos.Remove(instancia.Id);
			JsonUpdateMedicos(); // Save changes to the database
								 // MessageBox.Show($"Exito: Se ha eliminado el medico con id: {instancia.Id} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (Exception ex) {
			MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el medico", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;

		}
	}
	//------------------------public.DELETE.PacienteView----------------------//
	public override bool DeletePaciente(PacienteView instancia) {
		if (DictTurnos.Values.Any(i => i.PacienteId == instancia.Id)) {
			MessageBox.Show($"Error de integridad: El paciente tiene turnos asignados.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		try {
			DictPacientes.Remove(instancia.Id);
			JsonUpdatePacientes(); // Save changes to the database
								   // MessageBox.Show($"Exito: Se ha eliminado el paciente con id: {instancia.Id} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (Exception ex) {
			MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el paciente", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}
	//------------------------public.DELETE.TurnoView----------------------//
	public override bool DeleteTurno(TurnoView instancia) {
		try {
			DictTurnos.Remove(instancia.Id);
			JsonUpdateTurnos(); // Save changes to the database
								// MessageBox.Show($"Exito: Se ha eliminado el turno con id: {instancia.Id} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (Exception ex) {
			MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el turno", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}








	//------------------------private.LOAD.Medicos----------------------//
	private bool JsonCargarMedicosExitosamente(string file_path) {
		if (!File.Exists(file_path)) {
			MessageBox.Show($"Error: {file_path} no se encontró.", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		try {
			string jsonString = File.ReadAllText(file_path);
			var rawMedicosData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

			var medicos = new Dictionary<string, MedicoView>();

			foreach (var medicoEntry in rawMedicosData) {
				var jsonElement = System.Text.Json.JsonDocument.Parse(medicoEntry.Value.ToString()).RootElement;

				// --- Parsear días de atención ---
				var horariosCollection = new ObservableCollection<HorarioMedicoView>();

				if (jsonElement.TryGetProperty("DiasDeAtencion", out var diasJson)) {
					foreach (var diaProp in diasJson.EnumerateObject()) {
						var diaObj = diaProp.Value;

						string diaNombre = diaProp.Name;
						string? horaInicio = diaObj.GetProperty("HoraInicio").GetString();
						string? horaFin = diaObj.GetProperty("HoraFin").GetString();

						var diaConHorarios = new HorarioMedicoView {
							DiaName = diaNombre,
							FranjasHora = new ObservableCollection<HorarioMedicoTimeSpanView>()
						};

						// Agregar horario si ambos valores son válidos
						if (!string.IsNullOrWhiteSpace(horaInicio) && !string.IsNullOrWhiteSpace(horaFin)) {
							diaConHorarios.FranjasHora.Add(new HorarioMedicoTimeSpanView {
								Desde = TimeOnly.Parse(horaInicio),
								Hasta = TimeOnly.Parse(horaFin)
							});
						}

						horariosCollection.Add(diaConHorarios);
					}
				}

				// --- Crear instancia usando el constructor ---
				var medicoInstance = new MedicoView(
					horariosCollection,
					jsonElement.GetProperty(nameof(MedicoView.Id)).GetString() ?? medicoEntry.Key,
					jsonElement.GetProperty(nameof(MedicoView.Name)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(MedicoView.LastName)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(MedicoView.Dni)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(MedicoView.Provincia)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(MedicoView.Domicilio)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(MedicoView.Localidad)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(MedicoView.Especialidad)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(MedicoView.Telefono)).GetString() ?? string.Empty,
					jsonElement.TryGetProperty(nameof(MedicoView.Guardia), out var guardiaProp) && guardiaProp.ValueKind == System.Text.Json.JsonValueKind.True,
					DateTime.TryParse(
						jsonElement.GetProperty(nameof(MedicoView.FechaIngreso)).GetString(),
						out var fecha)
						? fecha
						: DateTime.MinValue,
					jsonElement.TryGetProperty(nameof(MedicoView.SueldoMinimoGarantizado), out var sueldoProp) && sueldoProp.ValueKind == System.Text.Json.JsonValueKind.Number
						? sueldoProp.GetDecimal()
						: 0
				);

				medicos[medicoEntry.Key] = medicoInstance;
			}

			DictMedicos = medicos;
		} catch (JsonException ex) {
			MessageBox.Show($"Error parseando el archivo Json {file_path}: {ex.Message}", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	//------------------------private.LOAD.Pacientes----------------------//
	private bool JsonCargarPacientesExitosamente(string file_path) {
		if (File.Exists(pacientesPath)) {
			try {
				string jsonString = File.ReadAllText(pacientesPath);
				DictPacientes = JsonConvert.DeserializeObject<Dictionary<string, PacienteView>>(jsonString);
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
				DictTurnos = JsonConvert.DeserializeObject<Dictionary<string, TurnoView>>(jsonString);
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
