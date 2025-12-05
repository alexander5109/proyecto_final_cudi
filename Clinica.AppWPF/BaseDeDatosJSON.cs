using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
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


	//------------------------public.CREATE.ViewModelMedico----------------------//
	public override bool CreateMedico(Medico2025 instancia, ViewModelMedico instanciaDto) {
		if (DictMedicos.Values.Any(i => i.Dni == instanciaDto.Dni)) {
			MessageBox.Show($"Error de integridad: Ya hay un medico con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		instanciaDto.Id = GenerateNextId(DictMedicos);
		DictMedicos[instanciaDto.Id] = instanciaDto;
		JsonUpdateMedicos();
		// MessageBox.Show($"Exito: Se ha creado la instancia de ViewModelMedico: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}
	//------------------------public.CREATE.ModelViewPaciente----------------------//
	public override bool CreatePaciente(Paciente2025 instancia, ViewModelPaciente instanciaDto) {
		if (DictPacientes.Values.Any(i => i.Dni == instanciaDto.Dni)) {
			MessageBox.Show($"Error de integridad: Ya hay un paciente con ese Dni.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		instanciaDto.Id = GenerateNextId(DictPacientes);
		DictPacientes[instanciaDto.Id] = instanciaDto;
		JsonUpdatePacientes();
		// MessageBox.Show($"Exito: Se ha creado la instancia de ModelViewPaciente: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}
	//------------------------public.CREATE.ModelViewTurno----------------------//
	public override bool CreateTurno(Turno2025 instancia, ViewModelTurno instanciaDto) {
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
		// MessageBox.Show($"Exito: Se ha creado la instancia de ModelViewTurno con CodigoInterno: {instancia.CodigoInterno}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}



	//public ModelViewPaciente(SystemTextJson.JsonElement json) {
	//	CodigoInterno = json.GetProperty(nameof(CodigoInterno)).GetString();
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
	public override List<ViewModelMedico> ReadMedicos() {
		return DictMedicos.Values.Cast<ViewModelMedico>().ToList();
	}
	public override List<ViewModelPaciente> ReadPacientes() {
		return DictPacientes.Values.Cast<ViewModelPaciente>().ToList();
	}
	public override List<ViewModelTurno> ReadTurnos() {
		return DictTurnos.Values.Cast<ViewModelTurno>().ToList();
	}










	//------------------------public.UPDATE.ViewModelMedico----------------------//
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
	//------------------------public.UPDATE.ModelViewPaciente----------------------//
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
	//------------------------public.UPDATE.ModelViewTurno----------------------//
	public override bool UpdateTurno(Turno2025 instanciaValidada, ViewModelTurno instancia) {
		if (DictTurnos.Values.Count(i => i.PacienteId == instancia.PacienteId && i.MedicoId == instancia.MedicoId && i.Fecha == instancia.Fecha) > 1) {
			MessageBox.Show($"Error de integridad: Ya hay un turno entre ese paciente y ese medico en esa fecha.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (DictTurnos.Values.Count(i => i.MedicoId == instancia.MedicoId && i.Fecha == instancia.Fecha && i.Hora == instancia.Hora) > 1) {
			MessageBox.Show($"Error de integridad: El medico ya tiene un turno ese dia a esa hora.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		JsonUpdateTurnos(); // Guardar los cambios en el archivo JSON
							// MessageBox.Show($"Exito: Se han actualizado los datos del turno CodigoInterno: {instancia.CodigoInterno}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
		return true;
	}











	//------------------------public.DELETE.ViewModelMedico----------------------//
	public override bool DeleteMedico(ViewModelMedico instancia) {
		if (DictTurnos.Values.Any(i => i.MedicoId == instancia.Id)) {
			MessageBox.Show($"Error de integridad: El medico tiene turnos asignados.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		try {
			DictMedicos.Remove(instancia.Id);
			JsonUpdateMedicos(); // Save changes to the database
								 // MessageBox.Show($"Exito: Se ha eliminado el medico con id: {instancia.CodigoInterno} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (Exception ex) {
			MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el medico", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;

		}
	}
	//------------------------public.DELETE.ModelViewPaciente----------------------//
	public override bool DeletePaciente(ViewModelPaciente instancia) {
		if (DictTurnos.Values.Any(i => i.PacienteId == instancia.Id)) {
			MessageBox.Show($"Error de integridad: El paciente tiene turnos asignados.\n No se guardarán los cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		try {
			DictPacientes.Remove(instancia.Id);
			JsonUpdatePacientes(); // Save changes to the database
								   // MessageBox.Show($"Exito: Se ha eliminado el paciente con id: {instancia.CodigoInterno} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (Exception ex) {
			MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el paciente", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}
	//------------------------public.DELETE.ModelViewTurno----------------------//
	public override bool DeleteTurno(ViewModelTurno instancia) {
		try {
			DictTurnos.Remove(instancia.Id);
			JsonUpdateTurnos(); // Save changes to the database
								// MessageBox.Show($"Exito: Se ha eliminado el turno con id: {instancia.CodigoInterno} del Json", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (Exception ex) {
			MessageBox.Show($"Error: {ex.Message}", "Error al querer eliminar el turno", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}








	//------------------------private.LOAD.WindowListarMedicos----------------------//
	private bool JsonCargarMedicosExitosamente(string file_path) {
		if (!File.Exists(file_path)) {
			MessageBox.Show($"Error: {file_path} no se encontró.", "Error JSON", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		try {
			string jsonString = File.ReadAllText(file_path);
			var rawMedicosData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

			var medicos = new Dictionary<string, ViewModelMedico>();

			foreach (var medicoEntry in rawMedicosData) {
				var jsonElement = System.Text.Json.JsonDocument.Parse(medicoEntry.Value.ToString()).RootElement;

				// --- Parsear días de atención ---
				var horariosCollection = new ObservableCollection<ViewModelHorario>();

				if (jsonElement.TryGetProperty("DiasDeAtencion", out var diasJson)) {
					foreach (var diaProp in diasJson.EnumerateObject()) {
						var diaObj = diaProp.Value;

						string diaNombre = diaProp.Name;
						string? horaInicio = diaObj.GetProperty("HoraInicio").GetString();
						string? horaFin = diaObj.GetProperty("HoraFin").GetString();

						DiaSemana2025 diaSemanaConvertido = default;
						// 1) Validamos el día
						Result<DiaSemana2025> diaSemanaResult = DiaSemana2025.Crear(diaNombre);

						// 1) Validamos el día usando Match
						diaSemanaResult.Switch(
							ok: diaSemanaValido => {
								// asumimos que DiaSemana2025 tiene algo como .ToDayOfWeek()
								diaSemanaConvertido = diaSemanaValido;
							},
							error: mensaje => {
								MessageBox.Show(
									$"Error: Día de la semana inválido '{diaNombre}' para el médico ID {medicoEntry.Key}. Se omitirá este día.",
									"Error de carga JSON",
									MessageBoxButton.OK,
									MessageBoxImage.Warning
								);
							}
						);

						// 2) Si hubo error, cortamos
						if (diaSemanaResult.IsError)
							throw new InvalidDataException();


						// 3) Construimos el objeto final
						var diaConHorarios = new ViewModelHorario {
							DiaSemana = diaSemanaConvertido.Valor,
							Desde = TimeOnly.Parse(horaInicio),
							Hasta = TimeOnly.Parse(horaFin),
						};

						horariosCollection.Add(diaConHorarios);
					}
				}

				// --- Programar instancia usando el constructor ---
				var medicoInstance = new ViewModelMedico(
					horariosCollection,
					jsonElement.GetProperty(nameof(ViewModelMedico.Id)).GetString() ?? medicoEntry.Key,
					jsonElement.GetProperty(nameof(ViewModelMedico.Name)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(ViewModelMedico.LastName)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(ViewModelMedico.Dni)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(ViewModelMedico.Provincia)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(ViewModelMedico.Domicilio)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(ViewModelMedico.Localidad)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(ViewModelMedico.EspecialidadCodigo)).GetString() ?? string.Empty,
					jsonElement.GetProperty(nameof(ViewModelMedico.Telefono)).GetString() ?? string.Empty,
					jsonElement.TryGetProperty(nameof(ViewModelMedico.Guardia), out var guardiaProp) && guardiaProp.ValueKind == System.Text.Json.JsonValueKind.True,
					DateTime.TryParse(
						jsonElement.GetProperty(nameof(ViewModelMedico.FechaIngreso)).GetString(),
						out var fecha)
						? fecha
						: DateTime.MinValue,
					jsonElement.TryGetProperty(nameof(ViewModelMedico.SueldoMinimoGarantizado), out var sueldoProp) && sueldoProp.ValueKind == System.Text.Json.JsonValueKind.Number
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

	//------------------------private.LOAD.WindowListarPacientes----------------------//
	private bool JsonCargarPacientesExitosamente(string file_path) {
		if (File.Exists(pacientesPath)) {
			try {
				string jsonString = File.ReadAllText(pacientesPath);
				DictPacientes = JsonConvert.DeserializeObject<Dictionary<string, ViewModelPaciente>>(jsonString);
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
	//------------------------private.LOAD.WindowListarTurnos----------------------//
	private bool JsonCargarTurnosExitosamente(string file_path) {
		if (File.Exists(turnosPath)) {
			try {
				string jsonString = File.ReadAllText(turnosPath);
				DictTurnos = JsonConvert.DeserializeObject<Dictionary<string, ViewModelTurno>>(jsonString);
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
	private int GenerateNextId<T>(Dictionary<int, T> dictionary) {
		int maxId = 0;

		foreach (var keyId in dictionary.Keys) {
			maxId = Math.Max(maxId, keyId);
		}
		return maxId + 1;
	}
	//------------------------settings----------------------//
	// settings
	//public override bool EliminarDatabaseExitosamente() {
	//return false;
	//}
	//------------------------Fin.BaseDeDatosJSON----------------------//
}
