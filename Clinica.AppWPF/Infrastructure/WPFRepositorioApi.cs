using System.Net.Http.Json;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.AppWPF.Infrastructure.IWPFRepositorioInterfaces;
using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.AppWPF.Infrastructure;


public class WPFRepositorioApi(ApiHelper Api) : IWPFRepositorio {

	async Task<List<MedicoDbModel>> IWPFRepositorioMedicos.SelectMedicos() {
		await EnsureMedicosLoaded();
		return [.. DictCacheMedicos.Values];
	}
	async Task<List<PacienteDbModel>> IWPFRepositorioPacientes.SelectPacientes() {
		await EnsurePacientesLoaded();
		return [.. DictCachePacientes.Values];
	}


	async Task<List<MedicoDbModel>> IWPFRepositorioMedicos.SelectMedicosWhereEspecialidadCodigo(EspecialidadCodigo code) {
		await EnsureMedicosLoaded();
		return [.. DictCacheMedicos
			.Values
			.Where(m => m.EspecialidadCodigo == code)];
		//return await Api.TryGetJsonAsync<List<MedicoDto>>($"api/medicos/por-especialidad/{code}", defaultValue: []);
	}



	async Task<MedicoDbModel?> IWPFRepositorioMedicos.SelectMedicoWhereId(MedicoId id) {
		await EnsureMedicosLoaded();

		if (DictCacheMedicos.TryGetValue(id, out MedicoDbModel? dto))
			return dto;
		MedicoDbModel? res = await Api.TryGetJsonOrNullAsync<MedicoDbModel>($"api/medicos/{id.Valor}");
		if (res is not null) {
			DictCacheMedicos[id] = res; // update cache
		}

		return res;
	}




	async Task<PacienteDbModel?> IWPFRepositorioPacientes.SelectPacienteWhereId(PacienteId id) {
		await EnsurePacientesLoaded();

		if (DictCachePacientes.TryGetValue(id, out PacienteDbModel? dto))
			return dto;
		PacienteDbModel? res = await Api.TryGetJsonOrNullAsync<PacienteDbModel>($"api/pacientes/{id.Valor}");
		if (res is not null) {
			DictCachePacientes[id] = res; // update cache
		}

		return res;
	}




	private Dictionary<MedicoId, MedicoDbModel> DictCacheMedicos = [];
	private Dictionary<PacienteId, PacienteDbModel> DictCachePacientes = [];



	private bool _medicosLoaded = false;
	private async Task EnsureMedicosLoaded() {
		if (_medicosLoaded)
			return;

        List<MedicoDbModel> list = await Api.TryGetJsonAsync<List<MedicoDbModel>>("api/medicos", defaultValue: []);

		DictCacheMedicos = list.ToDictionary(m => m.Id, m => m);
		foreach (KeyValuePair<MedicoId, MedicoDbModel> m in DictCacheMedicos ) {
			Console.WriteLine($"Medico cache loaded: {m.Key} -> {m.Value.Nombre} {m.Value.Apellido}");
		}
		_medicosLoaded = true;
	}


	private bool _pacientesLoaded = false;
	private async Task EnsurePacientesLoaded() {
		if (_pacientesLoaded)
			return;

        List<PacienteDbModel> list = await Api.TryGetJsonAsync<List<PacienteDbModel>>("api/pacientes", defaultValue: []);

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




	async Task<ResultWpf<UnitWpf>> IWPFRepositorioPacientes.UpdatePacienteWhereId(Paciente2025Agg aggrg) {
        ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/pacientes/{aggrg.Id.Valor}",
				aggrg.ToDto()
			),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error actualizando el agregado {aggrg.Id.Valor}"
		);

		_ = RefreshPacientes();

		return result;
	}
	async Task<ResultWpf<UnitWpf>> IWPFRepositorioMedicos.UpdateMedicoWhereId(Medico2025Agg aggrg) {
        ResultWpf<UnitWpf> result = await Api.TryApiCallAsync(
			() => Api.Cliente.PutAsJsonAsync(
				$"api/medicos/{aggrg.Id.Valor}",
				aggrg.ToDto()
			),
			onOk: async response => UnitWpf.Valor,   // Se ignora el body, pero se respeta la firma
			errorTitle: $"Error actualizando médico {aggrg.Id.Valor}"
		);

		_ = RefreshMedicos();

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

	Task<ResultWpf<TurnoDto>> IWPFRepositorioTurnos.AgendarNuevoTurno(PacienteId pacienteId, DateTime fechaSolicitudOriginal, Disponibilidad2025 disponibilidad) {
        throw new NotImplementedException();
    }

    Task<ResultWpf<TurnoDto>> IWPFRepositorioTurnos.CancelarTurno(TurnoId turnoId, DateTime fechaOutcome, string reason) {
        throw new NotImplementedException();
    }

    Task<ResultWpf<TurnoDto>> IWPFRepositorioTurnos.ReprogramarTurno(TurnoId turnoId, DateTime fechaOutcome, string reason) {
        throw new NotImplementedException();
    }

    Task<ResultWpf<TurnoDto>> IWPFRepositorioTurnos.MarcarTurnoComoAusente(TurnoId turnoId, DateTime fechaOutcome, string reason) {
        throw new NotImplementedException();
    }

    Task<ResultWpf<TurnoDto>> IWPFRepositorioTurnos.MarcarTurnoComoConcretado(TurnoId turnoId, DateTime fechaOutcome, string? reason) {
        throw new NotImplementedException();
    }
}
