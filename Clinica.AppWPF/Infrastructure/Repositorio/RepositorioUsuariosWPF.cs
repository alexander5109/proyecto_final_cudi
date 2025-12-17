using System.Net.Http.Json;
using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioUsuariosWPF : IRepositorioUsuariosWPF {
	public static Dictionary<UsuarioId2025, UsuarioDbModel> DictCache { get; set; } = [];


	private bool _usuariosLoaded = false;
	private async Task EnsureUsuariosLoaded() {
		if (_usuariosLoaded)
			return;

		List<UsuarioDbModel> list = await App.Api.TryGetJsonOrNullAsync<List<UsuarioDbModel>>("api/usuarios") ?? [];

		DictCache = list.ToDictionary(x => x.Id, x => x);
		_usuariosLoaded = true;
	}

	private async Task RefreshUsuarios() {
		_usuariosLoaded = false;
		await EnsureUsuariosLoaded();
	}

	async Task<List<UsuarioDbModel>> IRepositorioUsuariosWPF.SelectUsuarios() {
		await EnsureUsuariosLoaded();
		return [.. DictCache.Values];
	}
	Task<IReadOnlyCollection<AccionesDeUsuarioEnum>> IRepositorioUsuariosWPF.SelectAccionesDeUsuarioWhereEnumRole(UsuarioRoleEnum enumRole) {
		return Task.FromResult(ServiciosPublicos.GetAccionesDeUsuarioParaRol(enumRole));
	}

	Task<IReadOnlyCollection<AccionesDeUsuarioEnum>> IRepositorioUsuariosWPF.SelectAccionesDeUsuario() {
		return Task.FromResult(ServiciosPublicos.GetTodasLasAcciones());
	}
	async Task<ResultWpf<UnitWpf>> IRepositorioUsuariosWPF.DeleteUsuarioWhereId(
		UsuarioId2025 id
	) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			httpCall: () => App.Api.Cliente.DeleteAsync($"api/usuarios/{id.Valor}"),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error eliminando usuario {id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshUsuarios();
		}
		return result;
	}

	async Task<UsuarioDbModel?> IRepositorioUsuariosWPF.SelectUsuarioProfileWhereUsername(string username) {
		UsuarioDbModel? response = await App.Api.TryGetJsonOrNullAsync<UsuarioDbModel>($"api/usuarios/por-nombre/{username}");
		//MessageBox.Show($"{response?.ToString()}");
		return response;
	}
	async Task<ResultWpf<UnitWpf>> IRepositorioUsuariosWPF.UpdateUsuarioWhereId(Usuario2025EdicionAgg aggrg) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			httpCall: () => App.Api.Cliente.PutAsJsonAsync(
				$"api/usuarios/{aggrg.Id.Valor}",
				aggrg.ToModel()
			),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error actualizando usuario {aggrg.Id.Valor}"
		);
		if (result.IsOk) {
			_ = RefreshUsuarios();
		}
		return result;
	}
	async Task<ResultWpf<UsuarioId2025>> IRepositorioUsuariosWPF.InsertUsuarioReturnId(Usuario2025 instance) {
		ResultWpf<UsuarioId2025> result = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PostAsJsonAsync(
				"api/usuarios",
				instance.ToDto()
			),
			onOk: async response => {
				return await response.Content.ReadFromJsonAsync<UsuarioId2025>();
			},
			errorTitle: "Error creando usuario"
		);
		if (result.IsOk) {
			_ = RefreshUsuarios();
		}
		return result;
	}





}
