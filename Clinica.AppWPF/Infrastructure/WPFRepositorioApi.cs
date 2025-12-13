using System.Net.Http.Json;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Shared.ApiDtos;
using static Clinica.AppWPF.Infrastructure.IWPFRepositorioInterfaces;
using static Clinica.Shared.ApiDtos.PacienteDtos;
using static Clinica.Shared.ApiDtos.ServiciosPublicosDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure;

public static class RepoCache {
	public static Dictionary<MedicoId, MedicoDbModel> DictMedicos { get; set; } = [];
	public static Dictionary<PacienteId, PacienteDbModel> DictPacientes { get; set; } = [];
	public static Dictionary<UsuarioId, UsuarioDbModel> DictUsuarios { get; set; } = [];
	public static Dictionary<MedicoId, IReadOnlyList<HorarioDbModel>> DictHorarios { get; set; } = [];
}


public class WPFRepositorioApi(ApiHelper Api) : IWPFRepositorio {




	//public static async Task<PacienteDbModel> RespectivoPaciente(this PacienteId id) {
	//	PacienteDbModel? aggrg = await App.Repositorio.SelectPacienteWhereId(id);
	//	if (aggrg is not null) return aggrg;
	//	string error = $"No existe el médico con ID {id.Valor}";
	//	MessageBox.Show(error);
	//	throw new InvalidOperationException(error);
	//}







	//async Task<List<MedicoDbModel>> IWPFRepositorioMedicos.SelectMedicos() {
	//	await EnsureMedicosLoaded();
	//	return [.. RepoCache.DictMedicos.Values];
	//}

	async Task<List<MedicoDbModel>> IWPFRepositorioMedicos.SelectMedicos() {
		await EnsureMedicosLoaded();
		return [.. RepoCache.DictMedicos.Values];
	}


	//async Task<List<HorarioDbModel>> IWPFRepositorioHorarios.SelectHorarios() {
	//	await EnsureHorariosLoaded();
	//	return [.. RepoCache.DictHorarios.Values];
	//}



	async Task<IReadOnlyList<HorarioDbModel>?> IWPFRepositorioHorarios.SelectHorariosWhereMedicoId(MedicoId id) {
		await EnsureHorariosLoaded();
		return RepoCache.DictHorarios.GetValueOrDefault(id);
	}

	async Task<IReadOnlyList<DayOfWeek>?> IWPFRepositorioHorarios.SelectDiasDeAtencionWhereMedicoId(MedicoId id) {
		await EnsureHorariosLoaded();
		IReadOnlyList<HorarioDbModel>? resultad = RepoCache.DictHorarios.GetValueOrDefault(id);
		if (resultad is null) {
			return null;
		}
		return [.. resultad.Select(x => x.DiaSemana).Distinct()];
	}


	async Task<List<PacienteDbModel>> IWPFRepositorioPacientes.SelectPacientes() {
		await EnsurePacientesLoaded();
		return [.. RepoCache.DictPacientes.Values];
	}


	async Task<List<UsuarioDbModel>> IWPFRepositorioUsuarios.SelectUsuarios() {
		await EnsureUsuariosLoaded();
		return [.. RepoCache.DictUsuarios.Values];
	}



	async Task<List<MedicoDbModel>> IWPFRepositorioMedicos.SelectMedicosWhereEspecialidadCodigo(EspecialidadCodigo code) {
		await EnsureMedicosLoaded();
		return [.. RepoCache.DictMedicos
			.Values
			.Where(m => m.EspecialidadCodigo == code)];
		//return await Api.TryGetJsonAsync<List<MedicoDto>>($"api/medicos/por-especialidad/{code}", defaultValue: []);
	}



	async Task<MedicoDbModel?> IWPFRepositorioMedicos.SelectMedicoWhereId(MedicoId id) {
		await EnsureMedicosLoaded();

		if (RepoCache.DictMedicos.TryGetValue(id, out MedicoDbModel? dto))
			return dto;
		MedicoDbModel? res = await Api.TryGetJsonOrNullAsync<MedicoDbModel>($"api/medicos/{id.Valor}");
		//thriow devuevle horarios esto?
		if (res is not null) {
			RepoCache.DictMedicos[id] = res; // update cache
		}
		//medicos
		return res;
	}




	async Task<PacienteDbModel?> IWPFRepositorioPacientes.SelectPacienteWhereId(PacienteId id) {
		await EnsurePacientesLoaded();

		if (RepoCache.DictPacientes.TryGetValue(id, out PacienteDbModel? dto))
			return dto;
		PacienteDbModel? res = await Api.TryGetJsonOrNullAsync<PacienteDbModel>($"api/pacientes/{id.Valor}");
		if (res is not null) {
			RepoCache.DictPacientes[id] = res; // update cache
		}

		return res;
	}





	private bool _horariosLoaded = false;
	public async Task EnsureHorariosLoaded() {
		if (_horariosLoaded)
			return;
		List<HorarioDbModel> list =
			await Api.TryGetJsonAsync<List<HorarioDbModel>>(
				"api/horarios",
				defaultValue: []
			);
		RepoCache.DictHorarios = list
			.GroupBy(h => h.MedicoId)
			.ToDictionary(
				g => g.Key,
				g => (IReadOnlyList<HorarioDbModel>)[.. g]
			);

		_horariosLoaded = true;
	}






	private bool _medicosLoaded = false;
	public async Task EnsureMedicosLoaded() {
		if (_medicosLoaded)
			return;

		List<MedicoDbModel> list = await Api.TryGetJsonAsync<List<MedicoDbModel>>("api/medicos", defaultValue: []);

		RepoCache.DictMedicos.Clear();
		RepoCache.DictMedicos = list.ToDictionary(m => m.Id, m => m);
		//foreach (KeyValuePair<MedicoId, MedicoDbModel> m in DictMedicos) {
		//	Console.WriteLine($"Medico cache loaded: {m.Key} -> {m.Value.Nombre} {m.Value.Apellido}");
		//}
		_medicosLoaded = true;
	}


	private bool _pacientesLoaded = false;
	public async Task EnsurePacientesLoaded() {
		if (_pacientesLoaded)
			return;

		List<PacienteDbModel> list = await Api.TryGetJsonAsync<List<PacienteDbModel>>("api/pacientes", defaultValue: []);

		RepoCache.DictPacientes = list.ToDictionary(x => x.Id, x => x);
		_pacientesLoaded = true;
	}


	private bool _usuariosLoaded = false;
	private async Task EnsureUsuariosLoaded() {
		if (_usuariosLoaded)
			return;

		List<UsuarioDbModel> list = await Api.TryGetJsonOrNullAsync<List<UsuarioDbModel>>("api/usuarios") ?? [];

		RepoCache.DictUsuarios = list.ToDictionary(x => x.Id, x => x);
		_usuariosLoaded = true;
	}


	public async Task RefreshMedicos() {
		_medicosLoaded = false;
		await EnsureMedicosLoaded();
	}


	public async Task RefreshPacientes() {
		_pacientesLoaded = false;
		await EnsurePacientesLoaded();
	}

	public async Task RefreshUsuarios() {
		_usuariosLoaded = false;
		await EnsureUsuariosLoaded();
	}

	public async Task RefreshHorarios() {
		_horariosLoaded = false;
		await EnsureHorariosLoaded();
	}




	async Task<ResultWpf<PacienteId>> IWPFRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance) {
		ResultWpf<PacienteId> response = await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync(
				"api/pacientes",
				instance.ToDto()
			),
			onOk: async response => {
				return await response.Content.ReadFromJsonAsync<PacienteId>();
			},
			errorTitle: "Error creando paciente"
		);
		_ = RefreshPacientes();
		return response;
	}
	async Task<ResultWpf<MedicoId>> IWPFRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance) {
		ResultWpf<MedicoId> result = await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync(
				"api/medicos",
				instance.ToDto()
			),
			onOk: async response => {
				return await response.Content.ReadFromJsonAsync<MedicoId>();
			},
			errorTitle: "Error creando médico"
		);
		_ = RefreshMedicos();
		return result;
	}
	async Task<ResultWpf<UsuarioId>> IWPFRepositorioUsuarios.InsertUsuarioReturnId(Usuario2025 instance) {
		ResultWpf<UsuarioId> result = await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync(
				"api/medicos",
				instance.ToDto()
			),
			onOk: async response => {
				return await response.Content.ReadFromJsonAsync<UsuarioId>();
			},
			errorTitle: "Error creando usuario"
		);
		return result;
	}

















	async Task<List<TurnoDbModel>> IWPFRepositorioTurnos.SelectTurnosWherePacienteId(PacienteId id) {
		return await Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/pacientes/{id.Valor}/turnos"
		) ?? [];
	}

	async Task<List<TurnoDbModel>> IWPFRepositorioTurnos.SelectTurnosWhereMedicoId(MedicoId id) {
		return await Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/medicos/{id.Valor}/turnos"
		) ?? [];
	}

	async Task<List<TurnoDbModel>> IWPFRepositorioTurnos.SelectTurnos() {
		return await Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/turnos"
		) ?? [];
	}
	async Task<ResultWpf<UnitWpf>> IWPFRepositorioPacientes.DeletePacienteWhereId(PacienteId id) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			() => Api.Cliente.DeleteAsync($"api/pacientes/{id.Valor}"),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error eliminando paciente {id.Valor}"
		);

		_ = RefreshPacientes();

		return result;
	}

	async Task<ResultWpf<UnitWpf>> IWPFRepositorioMedicos.DeleteMedicoWhereId(MedicoId id) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			() => Api.Cliente.DeleteAsync($"api/medicos/{id.Valor}"),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error eliminando médico {id.Valor}"
		);

		_ = RefreshMedicos();

		return result;
	}




	async Task<ResultWpf<UnitWpf>> IWPFRepositorioPacientes.UpdatePacienteWhereId(
		Paciente2025Agg aggrg
	) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/pacientes/{aggrg.Id.Valor}",
				aggrg.ToModel()
			),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error actualizando el agregado {aggrg.Id.Valor}"
		);

		_ = RefreshPacientes();

		return result;
	}

	async Task<ResultWpf<UnitWpf>> IWPFRepositorioUsuarios.UpdateUsuarioWhereId(
		Usuario2025Agg aggrg
	) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			httpCall: () => Api.Cliente.PutAsJsonAsync(
				$"api/usuarios/{aggrg.Id.Valor}",
				aggrg.ToModel()
			),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error actualizando usuario {aggrg.Id.Valor}"
		);

		_ = RefreshUsuarios();
		return result;
	}






	async Task<List<Disponibilidad2025>> IWPFRepositorioDominio.SelectDisponibilidades(
		EspecialidadCodigo especialidad,
		int cuantos,
		DateTime apartirDeCuando
	) {
		string url =
			$"api/ServiciosPublicos/Turnos/Disponibilidades" +
			$"?EspecialidadCodigo={(byte)especialidad}" +
			$"&cuantos={cuantos}" +
			$"&aPartirDeCuando={apartirDeCuando:O}";

		return await Api.TryGetJsonAsync<List<Disponibilidad2025>>(url, defaultValue: []);
	}


	async Task<ResultWpf<UnitWpf>> IWPFRepositorioMedicos.UpdateMedicoWhereId(Medico2025Agg aggrg) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/medicos/{aggrg.Id.Valor}",
				aggrg.ToModel()
			),
			onOk: async response => UnitWpf.Valor,   // Se ignora el body, pero se respeta la firma
			errorTitle: $"Error actualizando médico {aggrg.Id.Valor}"
		);
		//_ = RefreshMedicos();
		return result;
	}



	async Task<ResultWpf<UnitWpf>> IWPFRepositorioTurnos.CancelarTurno(
		TurnoId turnoId,
		DateTime fechaOutcome,
		string? reason
	) {

		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync<UnitWpf>(
			httpCall: () => Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Cancelar",
				new ModificarTurnoDto(turnoId, fechaOutcome, reason)
			),
			errorTitle: $"Error al cancelar el turno {turnoId.Valor}"
		);
		//_ = RefreshMedicos();
		return result;
	}

	async Task<UsuarioDbModel?> IWPFRepositorioUsuarios.SelectUsuarioProfileWhereUsername(string username) {
        UsuarioDbModel? response = await Api.TryGetJsonOrNullAsync<UsuarioDbModel>($"api/usuarios/por-nombre/{username}");
		//MessageBox.Show($"{response?.ToString()}");
		return response;
	}

	async Task<ResultWpf<UnitWpf>> IWPFRepositorioTurnos.AgendarNuevoTurno(PacienteId pacienteId, DateTime fechaSolicitud, Disponibilidad2025 disponibilidad) {
		ResultWpf<UnitWpf> response = await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Programar",
				new ProgramarTurnoDto(
					pacienteId,
					fechaSolicitud,
					disponibilidad
				)
			),
			onOk: async response => {
				return UnitWpf.Valor;
				//return (await response.Content.ReadFromJsonAsync<TurnoDbModel>())!; //MMMMMM CODE SMELL
			},
			errorTitle: "Error agendando turno"
		);
		_ = RefreshPacientes();
		return response;
	}

	async Task<ResultWpf<UnitWpf>> IWPFRepositorioTurnos.ReprogramarTurno(
		TurnoId turnoId,
		DateTime fechaOutcome,
		string? reason
	) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			httpCall: () => Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Reprogramar",
				new ModificarTurnoDto(turnoId, fechaOutcome, reason)
			),
			onOk: async response => {
				return UnitWpf.Valor;
				//return (await response.Content.ReadFromJsonAsync<TurnoDbModel>())!;
			},
			errorTitle: $"Error reprogramando turno {turnoId.Valor}"
		);

		_ = RefreshMedicos();
		return result;
	}





	async Task<ResultWpf<UnitWpf>> IWPFRepositorioTurnos.MarcarTurnoComoAusente(
		TurnoId turnoId,
		DateTime fechaOutcome,
		string? reason
	) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			httpCall: () => Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/ConcretarComoAusente",
				new ModificarTurnoDto(turnoId, fechaOutcome, reason)
			),
			onOk: async response => {
				return UnitWpf.Valor;
				//return (await response.Content.ReadFromJsonAsync<TurnoDbModel>())!;
			},
			errorTitle: $"Error marcando como ausente el turno {turnoId.Valor}"
		);

		_ = RefreshMedicos();
		return result;
	}


	async Task<ResultWpf<UnitWpf>> IWPFRepositorioTurnos.MarcarTurnoComoConcretado(
		TurnoId turnoId,
		DateTime fechaOutcome,
		string? reason
	) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			httpCall: () => Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Concretar",
				new ConcretarTurnoDto(turnoId, fechaOutcome, reason)
			),
			onOk: async response => {
				return UnitWpf.Valor;
				//return (await response.Content.ReadFromJsonAsync<TurnoDbModel>())!;
			},
			errorTitle: $"Error concretando el turno {turnoId.Valor}"
		);

		_ = RefreshMedicos();
		return result;
	}


	async Task<ResultWpf<UnitWpf>> IWPFRepositorioUsuarios.DeleteUsuarioWhereId(
		UsuarioId id
	) {
		ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			httpCall: () => Api.Cliente.DeleteAsync($"api/usuarios/{id.Valor}"),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error eliminando usuario {id.Valor}"
		);

		_ = RefreshUsuarios();
		return result;
	}

}
