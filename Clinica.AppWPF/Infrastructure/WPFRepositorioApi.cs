using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.AppWPF.Infrastructure.IWPFRepositorioInterfaces;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;


public class WPFRepositorioApi(ApiHelper Api) : IWPFRepositorio {

	async Task<Result<Unit>> IWPFRepositorioPacientes.DeletePacienteWhereId(PacienteId id) {
		try {
			var response = await Api.Cliente.DeleteAsync($"api/pacientes/{id.Valor}");

			if (response.IsSuccessStatusCode)
				return new Result<Unit>.Ok(new Unit());

			return await ManejarErrorHttp<Unit>(response, $"Error eliminando paciente {id.Valor}");
		} catch (Exception ex) {
			return ManejarExcepcion<Unit>(ex, "Error de conexión al eliminar paciente");
		}
	}


	async Task<Result<PacienteId>> IWPFRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance) {
		try {
			var response = await Api.Cliente.PostAsJsonAsync("api/pacientes", instance.ToDto());

			if (!response.IsSuccessStatusCode)
				return await ManejarErrorHttp<PacienteId>(response, "Error creando paciente");

			var newId = await response.Content.ReadFromJsonAsync<int>();

			return new Result<PacienteId>.Ok(new PacienteId(newId));
		} catch (Exception ex) {
			return ManejarExcepcion<PacienteId>(ex, "Error de conexión al crear paciente");
		}
	}


	async Task<List<PacienteDto>> IWPFRepositorioPacientes.SelectPacientes() {
		try {
			var response = await Api.Cliente.GetFromJsonAsync<List<PacienteDto>>("api/pacientes");
			return response ?? [];
		} catch (Exception ex) {
			MessageBox.Show($"Error obteniendo pacientes:\n{ex.Message}",
				"Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return [];
		}
	}


	async Task<PacienteDto?> IWPFRepositorioPacientes.SelectPacienteWhereId(PacienteId id) {
		try {
			var response = await Api.Cliente.GetAsync($"api/pacientes/{id.Valor}");

			if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode) {
				await MostrarErrorHttp(response, $"Error obteniendo paciente {id.Valor}");
				return null;
			}

			return await response.Content.ReadFromJsonAsync<PacienteDto>();
		} catch (Exception ex) {
			MessageBox.Show($"Error obteniendo paciente:\n{ex.Message}",
				"Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return null;
		}
	}


	async Task<List<TurnoDto>> IWPFRepositorioPacientes.SelectTurnosWherePacienteId(PacienteId id) {
		try {
			var response = await Api.Cliente.GetAsync($"api/pacientes/{id.Valor}/turnos");

			if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				return [];

			if (!response.IsSuccessStatusCode) {
				await MostrarErrorHttp(response, $"Error obteniendo turnos del paciente {id.Valor}");
				return [];
			}

			var turnos =
				await response.Content.ReadFromJsonAsync<List<TurnoDto>>();

			return turnos ?? [];
		} catch (Exception ex) {
			MessageBox.Show($"Error obteniendo turnos:\n{ex.Message}",
				"Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return [];
		}
	}


	async Task<Result<Unit>> IWPFRepositorioPacientes.UpdatePacienteWhereId(Paciente2025 instance) {
		try {
			var response = await Api.Cliente.PutAsJsonAsync(
				$"api/pacientes/{instance.Id.Valor}",
				instance.ToDto()
			);

			if (response.IsSuccessStatusCode)
				return new Result<Unit>.Ok(new Unit());

			return await ManejarErrorHttp<Unit>(response, $"Error actualizando paciente {instance.Id.Valor}");
		} catch (Exception ex) {
			return ManejarExcepcion<Unit>(ex, "Error de conexión al actualizar paciente");
		}
	}



	// ===========================================================
	//     MÉTODOS COMUNES DE MANEJO DE ERRORES
	// ===========================================================

	static Result<T> ManejarExcepcion<T>(Exception ex, string titulo) {
		MessageBox.Show(
			$"{titulo}:\n{ex.Message}",
			"Error de conexión",
			MessageBoxButton.OK,
			MessageBoxImage.Error
		);

		return new Result<T>.Error(ex.Message);
	}


	static async Task<Result<T>> ManejarErrorHttp<T>(HttpResponseMessage response, string titulo) {
		string detalle = await response.Content.ReadAsStringAsync();

		switch (response.StatusCode) {

			case System.Net.HttpStatusCode.Unauthorized:
				MessageBox.Show(
					"No estás autenticado.\nPor favor iniciá sesión.",
					"Error: No Autorizado",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);
				return new Result<T>.Error("No autenticado");

			case System.Net.HttpStatusCode.Forbidden:
				MessageBox.Show(
					"No tenés permisos para realizar esta acción.",
					"Permiso denegado",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);
				return new Result<T>.Error("Permiso denegado");

			case System.Net.HttpStatusCode.NotFound:
				MessageBox.Show(
					"El recurso solicitado no existe.\n" + detalle,
					"No encontrado (404)",
					MessageBoxButton.OK,
					MessageBoxImage.Information
				);
				return new Result<T>.Error("Recurso no encontrado");

			case System.Net.HttpStatusCode.BadRequest:
				MessageBox.Show(
					"La solicitud es inválida.\n\n" + detalle,
					"Error de validación (400)",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);
				return new Result<T>.Error("Solicitud inválida");

			default:
				// Manejo genérico
				MessageBox.Show(
					$"{titulo}\nStatus: {(int)response.StatusCode}\n{detalle}",
					"Error HTTP",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				return new Result<T>.Error($"{titulo}: HTTP {(int)response.StatusCode}");
		}
	}


	static async Task MostrarErrorHttp(HttpResponseMessage response, string titulo) {
		string detalle = await response.Content.ReadAsStringAsync();

		MessageBox.Show(
			$"{titulo}\nStatus: {(int)response.StatusCode}\n{detalle}",
			"Error HTTP",
			MessageBoxButton.OK,
			MessageBoxImage.Warning
		);
	}










	// ===========================================================
	//       MÉDICOS
	// ===========================================================


	async Task<Result<Unit>> IWPFRepositorioMedicos.DeleteMedicoWhereId(MedicoId id) {
		try {
			var response = await Api.Cliente.DeleteAsync($"api/medicos/{id.Valor}");

			if (response.IsSuccessStatusCode)
				return new Result<Unit>.Ok(new Unit());

			return await ManejarErrorHttp<Unit>(response, $"Error eliminando médico {id.Valor}");
		} catch (Exception ex) {
			return ManejarExcepcion<Unit>(ex, "Error de conexión al eliminar médico");
		}
	}


	async Task<Result<MedicoId>> IWPFRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance) {
		try {
			var response = await Api.Cliente.PostAsJsonAsync("api/medicos", instance.ToDto());

			if (!response.IsSuccessStatusCode)
				return await ManejarErrorHttp<MedicoId>(response, "Error creando médico");

			var newId = await response.Content.ReadFromJsonAsync<int>();

			return new Result<MedicoId>.Ok(new MedicoId(newId));
		} catch (Exception ex) {
			return ManejarExcepcion<MedicoId>(ex, "Error de conexión al crear médico");
		}
	}


	async Task<Result<Unit>> IWPFRepositorioMedicos.UpdateMedicoWhereId(Medico2025 instance) {
		try {
			var response = await Api.Cliente.PutAsJsonAsync(
				$"api/medicos/{instance.Id.Valor}",
				instance.ToDto()
			);

			if (response.IsSuccessStatusCode)
				return new Result<Unit>.Ok(new Unit());

			return await ManejarErrorHttp<Unit>(response, $"Error actualizando médico {instance.Id.Valor}");
		} catch (Exception ex) {
			return ManejarExcepcion<Unit>(ex, "Error de conexión al actualizar médico");
		}
	}


	async Task<List<MedicoDto>> IWPFRepositorioMedicos.SelectMedicos() {
		try {
			var items = await Api.Cliente.GetFromJsonAsync<List<MedicoDto>>("api/medicos");
			return items ?? [];
		} catch (Exception ex) {
			MessageBox.Show(
				$"Error obteniendo médicos:\n{ex.Message}",
				"Error de conexión",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);
			return [];
		}
	}


	async Task<MedicoDto?> IWPFRepositorioMedicos.SelectMedicoWhereId(MedicoId id) {
		try {
			var response = await Api.Cliente.GetAsync($"api/medicos/{id.Valor}");

			if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode) {
				await MostrarErrorHttp(response, "Error obteniendo médico");
				return null;
			}

			return await response.Content.ReadFromJsonAsync<MedicoDto>();
		} catch (Exception ex) {
			MessageBox.Show(
				$"Error obteniendo médico:\n{ex.Message}",
				"Error de conexión",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);
			return null;
		}
	}


	async Task<List<TurnoDto>> IWPFRepositorioMedicos.SelectTurnosWhereMedicoId(MedicoId id) {
		try {
			var response = await Api.Cliente.GetAsync($"api/medicos/{id.Valor}/turnos");

			if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				return [];

			if (!response.IsSuccessStatusCode) {
				await MostrarErrorHttp(response, $"Error obteniendo turnos del médico {id.Valor}");
				return [];
			}

			var turnos = await response.Content.ReadFromJsonAsync<List<TurnoDto>>();
			return turnos ?? [];
		} catch (Exception ex) {
			MessageBox.Show(
				$"Error obteniendo turnos del médico:\n{ex.Message}",
				"Error de conexión",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);
			return [];
		}
	}


	async Task<List<MedicoDto>> IWPFRepositorioMedicos.SelectMedicosWhereEspecialidadCode(EspecialidadCodigo code) {
		try {
			var response = await Api.Cliente.GetAsync($"api/medicos/especialidad/{code}");

			if (!response.IsSuccessStatusCode) {
				await MostrarErrorHttp(response, $"Error obteniendo médicos de especialidad {code}");
				return [];
			}

			var medicos = await response.Content.ReadFromJsonAsync<List<MedicoDto>>();
			return medicos ?? [];
		} catch (Exception ex) {
			MessageBox.Show(
				$"Error obteniendo médicos por especialidad:\n{ex.Message}",
				"Error de conexión",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);
			return [];
		}
	}








}
