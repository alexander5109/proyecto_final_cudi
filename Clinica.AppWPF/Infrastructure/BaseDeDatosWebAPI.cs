using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Clinica.AppWPF.ViewModels;


namespace Clinica.AppWPF.Infrastructure;

public class BaseDeDatosWebAPI : BaseDeDatosInterface {
	async Task<List<WindowModificarPacienteViewModel>> BaseDeDatosInterface.ReadPacientes() {
		try {
			HttpResponseMessage response = await Api.Cliente.GetAsync("api/pacientes");
			if (!response.IsSuccessStatusCode) {
				string errorMsg = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error HTTP {(int)response.StatusCode}: {errorMsg}");
			}


			string rawJson = await response.Content.ReadAsStringAsync();
			File.WriteAllText("debug_pacientes.json", rawJson);

			//MessageBox.Show()
			List<WindowModificarPacienteViewModel>? data = await response.Content.ReadFromJsonAsync<List<WindowModificarPacienteViewModel>>();
			return data ?? [];
		} catch (Exception ex) {
			// Acá podés loguear
			MessageBox.Show($"Error al leer pacientes:\n{ex.Message}", "Error API", MessageBoxButton.OK);
			return [];
		}
	}



	Task<bool> BaseDeDatosInterface.CreateMedico(WindowModificarMedicoViewModel instance) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.CreatePaciente(WindowModificarPacienteViewModel instance) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.CreateTurno(WindowModificarTurnoViewModel instance) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.DeleteMedico(WindowModificarMedicoViewModel instance) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.DeletePaciente(WindowModificarPacienteViewModel instance) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.DeleteTurno(WindowModificarTurnoViewModel instance) {
		throw new NotImplementedException();
	}

	Task<EspecialidadMedicaViewModel> BaseDeDatosInterface.GetEspecialidadById(int id) {
		throw new NotImplementedException();
	}

	Task<WindowModificarMedicoViewModel> BaseDeDatosInterface.GetMedicoById(int id) {
		throw new NotImplementedException();
	}

	Task<WindowModificarPacienteViewModel> BaseDeDatosInterface.GetPacienteById(int id) {
		throw new NotImplementedException();
	}

	Task<WindowModificarTurnoViewModel> BaseDeDatosInterface.GetTurnoById(int id) {
		throw new NotImplementedException();
	}

	Task<List<EspecialidadMedicaViewModel>> BaseDeDatosInterface.ReadDistinctEspecialidades() {
		throw new NotImplementedException();
	}

	Task<List<WindowModificarMedicoViewModel>> BaseDeDatosInterface.ReadMedicos() {
		throw new NotImplementedException();
	}

	Task<List<WindowModificarMedicoViewModel>> BaseDeDatosInterface.ReadMedicosWhereEspecialidad(int? especialidadCodigoInterno) {
		throw new NotImplementedException();
	}
	Task<List<WindowModificarTurnoViewModel>> BaseDeDatosInterface.ReadTurnos() {
		throw new NotImplementedException();
	}

	Task<List<WindowModificarTurnoViewModel>> BaseDeDatosInterface.ReadTurnosWhereMedicoId(int? medicoId) {
		throw new NotImplementedException();
	}

	Task<List<WindowModificarTurnoViewModel>> BaseDeDatosInterface.ReadTurnosWherePacienteId(int? pacienteId) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.UpdateMedico(WindowModificarMedicoViewModel instance) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.UpdatePaciente(WindowModificarPacienteViewModel instance) {
		throw new NotImplementedException();
	}

	Task<bool> BaseDeDatosInterface.UpdateTurno(WindowModificarTurnoViewModel instance) {
		throw new NotImplementedException();
	}
}
