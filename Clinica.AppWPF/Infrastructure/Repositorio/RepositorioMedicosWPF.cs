using System.Net.Http.Json;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioMedicosWPF : IRepositorioMedicosWPF {
	
	public static Dictionary<MedicoId2025, MedicoDbModel> DictCache { get; set; } = [];
	

	private bool _medicosLoaded = false;
	public async Task EnsureMedicosLoaded() {
		if (_medicosLoaded)
			return;

		List<MedicoDbModel> list = await App.Api.TryGetJsonAsync<List<MedicoDbModel>>("api/medicos", defaultValue: []);

		DictCache.Clear();
		DictCache = list.ToDictionary(m => m.Id, m => m);
		//foreach (KeyValuePair<MedicoId2025, MedicoDbModel> m in DictMedicos) {
		//	Console.WriteLine($"Medico cache loaded: {m.Key} -> {m.Value.Nombre} {m.Value.Apellido}");
		//}
		_medicosLoaded = true;
	}


	public async Task RefreshCache() {
		_medicosLoaded = false;
		await EnsureMedicosLoaded();
	}












	

	async Task<List<MedicoDbModel>> IRepositorioMedicosWPF.SelectMedicosWhereEspecialidadCodigo(EspecialidadEnum code) {
		await EnsureMedicosLoaded();
		return [.. DictCache
			.Values
			.Where(m => m.EspecialidadCodigo == code)];
		//return await App.Api.TryGetJsonAsync<List<MedicoDto>>($"api/medicos/por-especialidad/{code}", defaultValue: []);
	}

	async Task<ResultWpf<UnitWpf>> IRepositorioMedicosWPF.DeleteMedicoWhereId(MedicoId2025 id) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.DeleteAsync($"api/medicos/{id.Valor}"),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error eliminando médico {id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshCache();
		}
		return result;
	}



	//async Task<List<MedicoDbModel>> IRepositorioMedicosWPF.SelectMedicos() {
	//	await EnsureMedicosLoaded();
	//	return [.. DictCache.Values];
	//}
	async Task<ResultWpf<UnitWpf>> IRepositorioMedicosWPF.UpdateMedicoWhereId(Medico2025Agg aggrg) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PutAsJsonAsync(
				$"api/medicos/{aggrg.Id.Valor}",
				aggrg.ToModel()
			),
			onOk: async response => UnitWpf.Valor,   // Se ignora el body, pero se respeta la firma
			errorTitle: $"Error actualizando médico {aggrg.Id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshCache();
		}
		return result;
	}


	async Task<ResultWpf<MedicoId2025>> IRepositorioMedicosWPF.InsertMedicoReturnId(Medico2025 instance) {
		ResultWpf<MedicoId2025> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PostAsJsonAsync(
				"api/medicos",
				instance.ToDto()
			),
			onOk: async response => {
				return await response.Content.ReadFromJsonAsync<MedicoId2025>();
			},
			errorTitle: "Error creando médico"
		);
		if (result.IsOk) {
			_ = RefreshCache();
		}
		return result;
	}

	async Task<MedicoDbModel?> IRepositorioMedicosWPF.SelectMedicoWhereId(MedicoId2025 id) {
		await EnsureMedicosLoaded();

		if (DictCache.TryGetValue(id, out MedicoDbModel? dto))
			return dto;
		MedicoDbModel? res = await App.Api.TryGetJsonOrNullAsync<MedicoDbModel>($"api/medicos/{id.Valor}");
		if (res is not null) {
			DictCache[id] = res;
		}
		return res;
	}

	string IRepositorioMedicosWPF.GetFromCacheMedicoDisplayWhereId(MedicoId2025 id) {
		// i don't do it. but consumer should // await RefreshCache();
		if (DictCache.TryGetValue(id, out MedicoDbModel? dto))
			return $"{dto.Nombre} {dto.Apellido}";
		return "Médico desconocido";
	}




	async Task<List<MedicoDbModel>> IRepositorioMedicosWPF.SelectMedicos() {
		await EnsureMedicosLoaded();
		return [.. DictCache.Values];
	}



	MedicoDbModel? IRepositorioMedicosWPF.GetFromCacheMedicoWhereId(MedicoId2025 id) {
		DictCache.TryGetValue(id, out MedicoDbModel? medico);
		return medico;
	}



}
