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
	private static Dictionary<PacienteId2025, PacienteDbModel> DictCache { get; set; } = [];


	private bool _pacientesLoaded = false;
	public async Task EnsurePacientesLoaded() {
		if (_pacientesLoaded)
			return;

		List<PacienteDbModel> list = await App.Api.TryGetJsonAsync<List<PacienteDbModel>>("api/pacientes", defaultValue: []);

		DictCache = list.ToDictionary(x => x.Id, x => x);
		_pacientesLoaded = true;
	}


	public async Task RefreshCache() {
		_pacientesLoaded = false;
		await EnsurePacientesLoaded();
	}



	public async Task<ResultWpf<UnitWpf>> DeletePacienteWhereId(PacienteId2025 id) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.DeleteAsync($"api/pacientes/{id.Valor}"),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error eliminando paciente {id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshCache();
		}
		return result;
	}




	public async Task<ResultWpf<PacienteId2025>> InsertPacienteReturnId(Paciente2025 instance) {
		ResultWpf<PacienteId2025> response = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PostAsJsonAsync(
				"api/pacientes",
				instance.ToDto()
			),
			onOk: async response => {
				return await response.Content.ReadFromJsonAsync<PacienteId2025>();
			},
			errorTitle: "Error creando paciente"
		);
		if (response.IsOk) {
			_ = RefreshCache();
		}
		return response;
	}




	public async Task<List<PacienteDbModel>> SelectPacientes() {
		await EnsurePacientesLoaded();
		return [.. DictCache.Values];
	}

	public async Task<PacienteDbModel?> SelectPacienteWhereId(PacienteId2025 id) {
		await EnsurePacientesLoaded();

		if (DictCache.TryGetValue(id, out PacienteDbModel? dto))
			return dto;
		PacienteDbModel? res = await App.Api.TryGetJsonOrNullAsync<PacienteDbModel>($"api/pacientes/{id.Valor}");
		if (res is not null) {
			DictCache[id] = res; // update cache
		}

		return res;
	}


	public async Task<ResultWpf<UnitWpf>> UpdatePacienteWhereId(Paciente2025Agg aggrg) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PutAsJsonAsync(
				$"api/pacientes/{aggrg.Id.Valor}",
				aggrg.ToModel()
			),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error actualizando el agregado {aggrg.Id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshCache();
		}

		return result;
	}






	PacienteDbModel? IRepositorioPacientesWPF.GetFromCachePacienteWhereId(PacienteId2025 id) {
		DictCache.TryGetValue(id, out var paciente);
		return paciente;
	}
}
