using System.Net.Http.Json;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioPacientesWPF : IRepositorioPacientesWPF {
	public static Dictionary<PacienteId, PacienteDbModel> CacheDict { get; set; } = [];
	

	private bool _pacientesLoaded = false;
	public async Task EnsurePacientesLoaded() {
		if (_pacientesLoaded)
			return;

		List<PacienteDbModel> list = await App.Api.TryGetJsonAsync<List<PacienteDbModel>>("api/pacientes", defaultValue: []);

		CacheDict = list.ToDictionary(x => x.Id, x => x);
		_pacientesLoaded = true;
	}


	private async Task RefreshPacientes() {
		_pacientesLoaded = false;
		await EnsurePacientesLoaded();
	}











	
	
	async Task<ResultWpf<UnitWpf>> IRepositorioPacientesWPF.DeletePacienteWhereId(PacienteId id) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.DeleteAsync($"api/pacientes/{id.Valor}"),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error eliminando paciente {id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshPacientes();
		}
		return result;
	}
	
	


	async Task<ResultWpf<PacienteId>> IRepositorioPacientesWPF.InsertPacienteReturnId(Paciente2025 instance) {
		ResultWpf<PacienteId> response = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PostAsJsonAsync(
				"api/pacientes",
				instance.ToDto()
			),
			onOk: async response => {
				return await response.Content.ReadFromJsonAsync<PacienteId>();
			},
			errorTitle: "Error creando paciente"
		);
		if (response.IsOk) {
			_ = RefreshPacientes();
		}
		return response;
	}




	async Task<List<PacienteDbModel>> IRepositorioPacientesWPF.SelectPacientes() {
		await EnsurePacientesLoaded();
		return [.. CacheDict.Values];
	}

	async Task<PacienteDbModel?> IRepositorioPacientesWPF.SelectPacienteWhereId(PacienteId id) {
		await EnsurePacientesLoaded();

		if (CacheDict.TryGetValue(id, out PacienteDbModel? dto))
			return dto;
		PacienteDbModel? res = await App.Api.TryGetJsonOrNullAsync<PacienteDbModel>($"api/pacientes/{id.Valor}");
		if (res is not null) {
			CacheDict[id] = res; // update cache
		}

		return res;
	}


	async Task<ResultWpf<UnitWpf>> IRepositorioPacientesWPF.UpdatePacienteWhereId(Paciente2025Agg aggrg) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PutAsJsonAsync(
				$"api/pacientes/{aggrg.Id.Valor}",
				aggrg.ToModel()
			),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error actualizando el agregado {aggrg.Id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshPacientes();
		}

		return result;
	}
	
	
}
