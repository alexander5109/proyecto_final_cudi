using System.Net.Http.Json;
using System.Windows;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.AppWPF.Infrastructure.IWPFRepositorioInterfaces;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Infrastructure;


public class WPFRepositorioApi(ApiHelper Api) : IWPFRepositorio {

	async Task<List<MedicoDto>> IWPFRepositorioMedicos.SelectMedicos() {
		await EnsureMedicosLoaded();
		return [.. DictCacheMedicos.Values];
	}
	async Task<List<PacienteDto>> IWPFRepositorioPacientes.SelectPacientes() {
		await EnsurePacientesLoaded();
		return [.. DictCachePacientes.Values];
	}


	async Task<List<MedicoDto>> IWPFRepositorioMedicos.SelectMedicosWhereEspecialidadCodigo(EspecialidadCodigo code) {
		await EnsureMedicosLoaded();
		return DictCacheMedicos
			.Values
			.Where(m => m.EspecialidadCodigo == code)
			.ToList();
		//return await Api.TryGetJsonAsync<List<MedicoDto>>($"api/medicos/por-especialidad/{code}", defaultValue: []);
	}



	async Task<MedicoDto?> IWPFRepositorioMedicos.SelectMedicoWhereId(MedicoId id) {
		await EnsureMedicosLoaded();

		if (DictCacheMedicos.TryGetValue(id, out var dto))
			return dto;
		var res = await Api.TryGetJsonOrNullAsync<MedicoDto>($"api/medicos/{id.Valor}");
		if (res is not null) {
			DictCacheMedicos[id] = res; // update cache
		}

		return res;
	}




	async Task<PacienteDto?> IWPFRepositorioPacientes.SelectPacienteWhereId(PacienteId id) {
		await EnsurePacientesLoaded();

		if (DictCachePacientes.TryGetValue(id, out var dto))
			return dto;
		var res = await Api.TryGetJsonOrNullAsync<PacienteDto>($"api/pacientes/{id.Valor}");
		if (res is not null) {
			DictCachePacientes[id] = res; // update cache
		}

		return res;
	}




	private Dictionary<MedicoId, MedicoDto> DictCacheMedicos = [];
	private Dictionary<PacienteId, PacienteDto> DictCachePacientes = [];



	private bool _medicosLoaded = false;
	private async Task EnsureMedicosLoaded() {
		if (_medicosLoaded)
			return;

		var list = await Api.TryGetJsonAsync<List<MedicoDto>>("api/medicos", defaultValue: []);

		DictCacheMedicos = list.ToDictionary(m => m.Id, m => m);
		foreach ( var m in DictCacheMedicos ) {
			Console.WriteLine($"Medico cache loaded: {m.Key} -> {m.Value.Nombre} {m.Value.Apellido}");
		}
		_medicosLoaded = true;
	}


	private bool _pacientesLoaded = false;
	private async Task EnsurePacientesLoaded() {
		if (_pacientesLoaded)
			return;

		var list = await Api.TryGetJsonAsync<List<PacienteDto>>("api/pacientes", defaultValue: []);

		DictCachePacientes = list.ToDictionary(x => x.Id, x => x);
		_pacientesLoaded = true;
	}


	public async Task RefreshMedicos() {
		_medicosLoaded = false;
		await EnsureMedicosLoaded();
	}

	public async Task RefreshPacientes() {
		_medicosLoaded = false;
		await EnsurePacientesLoaded();
	}




	async Task<Result<PacienteId>> IWPFRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance) {
		var response = await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync(
				"api/pacientes", 
				instance.ToDto()
			),
			onOk: async response => {
				int id = await response.Content.ReadFromJsonAsync<int>();
				return new PacienteId(id);
			},
			errorTitle: "Error creando paciente"
		);
		_ = RefreshPacientes();
		return response;
	}
	async Task<Result<MedicoId>> IWPFRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance) {
		var result = await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync(
				"api/medicos", 
				instance.ToDto()
			),
			onOk: async response => {
				int id = await response.Content.ReadFromJsonAsync<int>();
				return new MedicoId(id);
			},
			errorTitle: "Error creando médico"
		);
		_ = RefreshMedicos();
		return result;
	}




	async Task<Result<Unit>> IWPFRepositorioPacientes.UpdatePacienteWhereId(Paciente2025 instance) {
		var result = await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/pacientes/{instance.Id.Valor}",
				instance.ToDto()
			),
			onOk: async response => new Unit(),
			errorTitle: $"Error actualizando paciente {instance.Id.Valor}"
		);

		_ = RefreshPacientes();

		return result;
	}
	async Task<Result<Unit>> IWPFRepositorioMedicos.UpdateMedicoWhereId(Medico2025 instance) {
		var result = await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/medicos/{instance.Id.Valor}",
				instance.ToDto()
			),
			onOk: async response => new Unit(),   // Se ignora el body, pero se respeta la firma
			errorTitle: $"Error actualizando médico {instance.Id.Valor}"
		);

		_ = RefreshMedicos();

		return result;
	}




	async Task<List<Disponibilidad2025>> IWPFRepositorioDominio.SelectDisponibilidades(
		EspecialidadCodigo especialidad,
		int cuantos,
		DateTime apartirDeCuando
	) {

		//[HttpGet("Turnos/Disponibilidades")]
		string url =
			$"api/ServiciosPublicos/Turnos/Disponibilidades" +
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
















	async Task<Result<Unit>> IWPFRepositorioPacientes.DeletePacienteWhereId(PacienteId id) {
		var result = await Api.TryApiCallAsync(
			() => Api.Cliente.DeleteAsync($"api/pacientes/{id.Valor}"),
			onOk: async response => new Unit(),
			errorTitle: $"Error eliminando paciente {id.Valor}"
		);

		_ = RefreshPacientes();

		return result;
	}

	async Task<Result<Unit>> IWPFRepositorioMedicos.DeleteMedicoWhereId(MedicoId id) {
		var result = await Api.TryApiCallAsync(
			() => Api.Cliente.DeleteAsync($"api/medicos/{id.Valor}"),
			onOk: async response => new Unit(),
			errorTitle: $"Error eliminando médico {id.Valor}"
		);

		_ = RefreshMedicos();

		return result;
	}

}
