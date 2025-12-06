using System.Net.Http.Json;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.AppWPF.Infrastructure.IWPFRepositorioInterfaces;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;


public class WPFRepositorioApi(ApiHelper Api) : IWPFRepositorio {

	async Task<Result<Unit>> IWPFRepositorioPacientes.DeletePacienteWhereId(PacienteId id) {
		return await Api.TryApiCallAsync(
			() => Api.Cliente.DeleteAsync($"api/pacientes/{id.Valor}"),
			onOk: () => new Unit(),
			errorTitle: $"Error eliminando paciente {id.Valor}"
		);
	}

	async Task<Result<PacienteId>> IWPFRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance) {
		return await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync("api/pacientes", instance.ToDto()),
			onOk: () => {
                int newId = Api.Cliente
					.PostAsJsonAsync("api/pacientes", instance.ToDto())
					.Result.Content.ReadFromJsonAsync<int>().Result;
				return new PacienteId(newId);
			},
			errorTitle: "Error creando paciente"
		);
	}

	async Task<List<PacienteDto>> IWPFRepositorioPacientes.SelectPacientes() {
		return await Api.TryGetJsonAsync<List<PacienteDto>>("api/pacientes", defaultValue: []);
	}


	async Task<PacienteDto?> IWPFRepositorioPacientes.SelectPacienteWhereId(PacienteId id) {
		return await Api.TryGetJsonOrNullAsync<PacienteDto>($"api/pacientes/{id.Valor}");
	}




	async Task<Result<Unit>> IWPFRepositorioPacientes.UpdatePacienteWhereId(Paciente2025 instance) {
		return await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/pacientes/{instance.Id.Valor}",
				instance.ToDto()
			),
			onOk: () => new Unit(),
			errorTitle: $"Error actualizando paciente {instance.Id.Valor}"
		);
	}



	async Task<Result<Unit>> IWPFRepositorioMedicos.DeleteMedicoWhereId(MedicoId id) {
		return await Api.TryApiCallAsync(
			() => Api.Cliente.DeleteAsync($"api/medicos/{id.Valor}"),
			onOk: () => new Unit(),
			errorTitle: $"Error eliminando médico {id.Valor}"
		);
	}


	async Task<Result<MedicoId>> IWPFRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance) {
		return await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync("api/medicos", instance.ToDto()),
			onOk: () => {
				// OJO: esto es síncrono dentro de onOk, pero coherente con tu diseño actual
				int newId = Api.Cliente
					.PostAsJsonAsync("api/medicos", instance.ToDto())
					.Result.Content.ReadFromJsonAsync<int>().Result;

				return new MedicoId(newId);
			},
			errorTitle: "Error creando médico"
		);
	}



	async Task<Result<Unit>> IWPFRepositorioMedicos.UpdateMedicoWhereId(Medico2025 instance) {
		return await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/medicos/{instance.Id.Valor}",
				instance.ToDto()
			),
			onOk: () => new Unit(),
			errorTitle: $"Error actualizando médico {instance.Id.Valor}"
		);
	}


	async Task<List<MedicoDto>> IWPFRepositorioMedicos.SelectMedicos() {
		return await Api.TryGetJsonAsync<List<MedicoDto>>("api/medicos", defaultValue: []);
	}




	async Task<List<MedicoDto>> IWPFRepositorioMedicos.SelectMedicosWhereEspecialidadCodigo(EspecialidadCodigo code) {
		return await Api.TryGetJsonAsync<List<MedicoDto>>($"api/medicos/por-especialidad/{code}", defaultValue: []
		);
	}


	async Task<MedicoDto?> IWPFRepositorioMedicos.SelectMedicoWhereId(MedicoId id) {
		return await Api.TryGetJsonOrNullAsync<MedicoDto>(
			$"api/medicos/{id.Valor}"
		);
	}



	async Task<List<Disponibilidad2025>> IWPFRepositorioDominio.SelectDisponibilidades(
		EspecialidadCodigo especialidad,
		int cuantos,
		DateTime apartirDeCuando
	) {
		string url =
			$"api/ServiciosPublicos/Turnos/DisponibilidadesItemsSource" +
			$"?EspecialidadCodigo={(byte)especialidad}" +
			$"&cuantos={cuantos}" +
			$"&aPartirDeCuando={apartirDeCuando:O}";

		return await Api.TryGetJsonAsync<List<Disponibilidad2025>>(url, defaultValue: []);
	}




	async Task<List<TurnoDto>> IWPFRepositorioTurnos.SelectTurnosWherePacienteId(PacienteId id) {
		return await Api.TryGetJsonOrNullAsync<List<TurnoDto>>(
			$"api/pacientes/{id.Valor}/turnos"
		) ?? [];
	}


	async Task<List<TurnoDto>> IWPFRepositorioTurnos.SelectTurnosWhereMedicoId(MedicoId id) {
		return await Api.TryGetJsonOrNullAsync<List<TurnoDto>>(
			$"api/medicos/{id.Valor}/turnos"
		) ?? [];
	}

	async Task<List<TurnoDto>> IWPFRepositorioTurnos.SelectTurnos() {
		return await Api.TryGetJsonOrNullAsync<List<TurnoDto>>(
			$"api/turnos"
		) ?? [];
	}

}
