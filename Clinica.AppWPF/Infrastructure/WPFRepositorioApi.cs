using System.Net.Http.Json;
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
	public static Dictionary<MedicoId, MedicoDbModel> DictMedicos { get; set; } = new();
	public static Dictionary<PacienteId, PacienteDbModel> DictPacientes { get; set; } = new();
}


public class WPFRepositorioApi(ApiHelper Api) : IWPFRepositorio {




	//public static async Task<PacienteDbModel> RespectivoPaciente(this PacienteId id) {
	//	PacienteDbModel? instance = await App.Repositorio.SelectPacienteWhereId(id);
	//	if (instance is not null) return instance;
	//	string error = $"No existe el médico con ID {id.Valor}";
	//	MessageBox.Show(error);
	//	throw new InvalidOperationException(error);
	//}







	async Task<List<MedicoDbModel>> IWPFRepositorioMedicos.SelectMedicosWithHorarios() {
		await EnsureMedicosLoaded();
		return [.. RepoCache.DictMedicos.Values];
	}

	async Task<List<PacienteDbModel>> IWPFRepositorioPacientes.SelectPacientes() {
		await EnsurePacientesLoaded();
		return [.. RepoCache.DictPacientes.Values];
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
		if (res is not null) {
			RepoCache.DictMedicos[id] = res; // update cache
		}

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





	private bool _medicosLoaded = false;
	private async Task EnsureMedicosLoaded() {
		if (_medicosLoaded)
			return;

		List<MedicoDbModel> list = await Api.TryGetJsonAsync<List<MedicoDbModel>>("api/medicos/con-horarios", defaultValue: []);

		RepoCache.DictMedicos.Clear();
		RepoCache.DictMedicos = list.ToDictionary(m => m.Id, m => m);
		//foreach (KeyValuePair<MedicoId, MedicoDbModel> m in DictMedicos ) {
		//	Console.WriteLine($"Medico cache loaded: {m.Key} -> {m.Value.Nombre} {m.Value.Apellido}");
		//}
		_medicosLoaded = true;
	}


	private bool _pacientesLoaded = false;
	private async Task EnsurePacientesLoaded() {
		if (_pacientesLoaded)
			return;

		List<PacienteDbModel> list = await Api.TryGetJsonAsync<List<PacienteDbModel>>("api/pacientes", defaultValue: []);

		RepoCache.DictPacientes = list.ToDictionary(x => x.Id, x => x);
		_pacientesLoaded = true;
	}


	public async Task RefreshMedicos() {
		_medicosLoaded = false;
		await EnsureMedicosLoaded();
	}


	public async Task RefreshPacientes() {
		_pacientesLoaded = false;
		await EnsurePacientesLoaded();
	}




	async Task<ResultWpf<PacienteId>> IWPFRepositorioPacientes.InsertPacienteReturnId(Paciente2025 instance) {
		ResultWpf<PacienteId> response = await Api.TryApiCallAsync(
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
	async Task<ResultWpf<MedicoId>> IWPFRepositorioMedicos.InsertMedicoReturnId(Medico2025 instance) {
		ResultWpf<MedicoId> result = await Api.TryApiCallAsync(
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
	async Task<ResultWpf<UsuarioId>> IWPFRepositorioUsuarios.InsertUsuarioReturnId(Usuario2025 instance) {
		ResultWpf<UsuarioId> result = await Api.TryApiCallAsync(
			() => Api.Cliente.PostAsJsonAsync(
				"api/medicos",
				instance.ToDto()
			),
			onOk: async response => {
				int id = await response.Content.ReadFromJsonAsync<int>();
				return new UsuarioId(id);
			},
			errorTitle: "Error creando usuario"
		);
		return result;
	}




	async Task<ResultWpf<UnitWpf>> IWPFRepositorioPacientes.UpdatePacienteWhereId(Paciente2025Agg aggrg) {
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

		_ = RefreshMedicos();

		return result;
	}



	async Task<ResultWpf<TurnoDbModel>> IWPFRepositorioTurnos.CancelarTurno(
		TurnoId turnoId,
		DateTime fechaOutcome,
		string reason
	) {

		ResultWpf<TurnoDbModel> result = await Api.TryApiCallAsync<TurnoDbModel>(
			httpCall: () => Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Cancelar",
				new ModificarTurnoDto(turnoId, fechaOutcome, reason)
			),
			errorTitle: $"Error al cancelar el turno {turnoId.Valor}"
		);


		_ = RefreshMedicos();
		return result;
	}

	async Task<ResultWpf<TurnoDbModel>> IWPFRepositorioTurnos.AgendarNuevoTurno(PacienteId pacienteId, DateTime fechaSolicitudOriginal, Disponibilidad2025 disponibilidad) {
		throw new NotImplementedException();
	}
	async Task<ResultWpf<TurnoDbModel>> IWPFRepositorioTurnos.ReprogramarTurno(TurnoId turnoId, DateTime fechaOutcome, string? reason) {
		//if (string.IsNullOrEmpty(reason)) {
		//return new ResultWpf<TurnoDbModel>.Error("Marcar un turno como reprogramado requiere un comentario con motivo.");
		throw new NotImplementedException();
	}

	async Task<ResultWpf<TurnoDbModel>> IWPFRepositorioTurnos.MarcarTurnoComoAusente(TurnoId turnoId, DateTime fechaOutcome, string? reason) {
		//if (string.IsNullOrEmpty(reason)) {
		//return new ResultWpf<TurnoDbModel>.Error("Marcar un turno como ausente requiere un comentario con motivo.");

		throw new NotImplementedException();
	}

	async Task<ResultWpf<TurnoDbModel>> IWPFRepositorioTurnos.MarcarTurnoComoConcretado(TurnoId turnoId, DateTime fechaOutcome, string? reason) {
		throw new NotImplementedException();
	}

	async Task<ResultWpf<UnitWpf>> IWPFRepositorioUsuarios.DeleteUsuarioWhereId(UsuarioId id) {
		throw new NotImplementedException();
	}

	async Task<ResultWpf<UnitWpf>> IWPFRepositorioUsuarios.UpdateUsuarioWhereId(Usuario2025Agg instance) {
		throw new NotImplementedException();
	}

	async Task<List<UsuarioDbModel>> IWPFRepositorioUsuarios.SelectUsuarios() {
		throw new NotImplementedException();
	}

	async Task<UsuarioDto?> IWPFRepositorioUsuarios.SelectUsuarioProfileWhereUsername(UserName username) {
		return await Api.TryGetJsonOrNullAsync<UsuarioDto>($"api/usuarios/{username.Valor}");
	}
}
