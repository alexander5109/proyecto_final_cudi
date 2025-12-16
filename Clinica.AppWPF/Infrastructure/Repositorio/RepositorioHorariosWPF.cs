using System.Net.Http.Json;
using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioHorariosWPF : IRepositorioHorariosWPF {
	private static Dictionary<MedicoId, IReadOnlyList<HorarioDbModel>> DictCache { get; set; } = [];
	
	
	


	private bool _horariosLoaded = false;
	public async Task EnsureHorariosLoaded() {
		if (_horariosLoaded)
			return;
		List<HorarioDbModel> list =
			await App.Api.TryGetJsonAsync<List<HorarioDbModel>>(
				"api/horarios",
				defaultValue: []
			);
		DictCache = list
			.GroupBy(h => h.MedicoId)
			.ToDictionary(
				g => g.Key,
				g => (IReadOnlyList<HorarioDbModel>)[.. g]
			);

		_horariosLoaded = true;
	}

	private async Task RefreshHorarios() {
		_horariosLoaded = false;
		await EnsureHorariosLoaded();
	}
	
	
	
	
	
	async Task<ResultWpf<UnitWpf>> IRepositorioHorariosWPF.UpdateHorariosWhereMedicoId(HorariosMedicos2026Agg agregado) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PutAsJsonAsync(
				$"api/horarios/{agregado.MedicoId.Valor}",
				agregado.Franjas
			),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error actualizando médico {agregado.MedicoId.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshHorarios();
		}
		return result;
	}


	//async Task<List<HorarioDbModel>> IRepositorioHorariosWPF.SelectHorarios() {
	//	await EnsureHorariosLoaded();
	//	return [.. RepoCache.DictCache.Values];
	//}



	async Task<IReadOnlyList<HorarioDbModel>?> IRepositorioHorariosWPF.SelectHorariosWhereMedicoId(MedicoId id) {
		await EnsureHorariosLoaded();
		return DictCache.GetValueOrDefault(id);
	}

	async Task<IReadOnlyList<DayOfWeek>?> IRepositorioHorariosWPF.SelectDiasDeAtencionWhereMedicoId(MedicoId id) {
		await EnsureHorariosLoaded();
		IReadOnlyList<HorarioDbModel>? resultad = DictCache.GetValueOrDefault(id);
		if (resultad is null) {
			return null;
		}
		return [.. resultad.Select(x => x.DiaSemana).Distinct().OrderBy(d => d)];
	}




}
