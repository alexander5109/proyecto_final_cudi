using System.Net.Http.Json;
using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.ApiDtos.ServiciosPublicosDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioTurnosWPF : IRepositorioTurnosWPF {
	
	
	
	

	async Task<List<TurnoDbModel>> IRepositorioTurnosWPF.SelectTurnosWherePacienteId(PacienteId2025 id) {
		return await App.Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/pacientes/{id.Valor}/turnos"
		) ?? [];
	}

	async Task<List<TurnoDbModel>> IRepositorioTurnosWPF.SelectTurnosWhereMedicoId(MedicoId2025 id) {
		return await App.Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/medicos/{id.Valor}/turnos"
		) ?? [];
	}

	async Task<List<TurnoDbModel>> IRepositorioTurnosWPF.SelectTurnos() {
		return await App.Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/turnos"
		) ?? [];
	}




	async Task<ResultWpf<UnitWpf>> IRepositorioTurnosWPF.CancelarTurno(
		TurnoId2025 turnoId,
		DateTime fechaOutcome,
		string? reason
	) {

		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync<UnitWpf>(
			httpCall: () => App.Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Cancelar",
				new ModificarTurnoDto(turnoId, fechaOutcome, reason)
			),
			errorTitle: $"Error al cancelar el turno {turnoId.Valor}"
		);
		//_ = RefreshCache();
		return result;
	}


	async Task<ResultWpf<UnitWpf>> IRepositorioTurnosWPF.AgendarNuevoTurno(PacienteId2025 pacienteId, DateTime fechaSolicitud, Disponibilidad2025 disponibilidad) {
		ResultWpf<UnitWpf> response = await App.Api.TryApiCallAsync(
			() => App.Api.Cliente.PostAsJsonAsync(
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
		//_ = RefreshCache();
		return response;
	}

	async Task<ResultWpf<UnitWpf>> IRepositorioTurnosWPF.ReprogramarTurno(
		TurnoId2025 turnoId,
		DateTime fechaOutcome,
		string? reason
	) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			httpCall: () => App.Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Reprogramar",
				new ModificarTurnoDto(turnoId, fechaOutcome, reason)
			),
			onOk: async response => {
				return UnitWpf.Valor;
				//return (await response.Content.ReadFromJsonAsync<TurnoDbModel>())!;
			},
			errorTitle: $"Error reprogramando turno {turnoId.Valor}"
		);

		//_ = RefreshCache();
		return result;
	}





	async Task<ResultWpf<UnitWpf>> IRepositorioTurnosWPF.MarcarTurnoComoAusente(
		TurnoId2025 turnoId,
		DateTime fechaOutcome,
		string? reason
	) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			httpCall: () => App.Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/ConcretarComoAusente",
				new ModificarTurnoDto(turnoId, fechaOutcome, reason)
			),
			onOk: async response => {
				return UnitWpf.Valor;
				//return (await response.Content.ReadFromJsonAsync<TurnoDbModel>())!;
			},
			errorTitle: $"Error marcando como ausente el turno {turnoId.Valor}"
		);

		//_ = RefreshCache();
		return result;
	}


	async Task<ResultWpf<UnitWpf>> IRepositorioTurnosWPF.MarcarTurnoComoConcretado(
		TurnoId2025 turnoId,
		DateTime fechaOutcome,
		string? reason
	) {
		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			httpCall: () => App.Api.Cliente.PutAsJsonAsync(
				"api/ServiciosPublicos/Turnos/Concretar",
				new ConcretarTurnoDto(turnoId, fechaOutcome, reason)
			),
			onOk: async response => {
				return UnitWpf.Valor;
				//return (await response.Content.ReadFromJsonAsync<TurnoDbModel>())!;
			},
			errorTitle: $"Error concretando el turno {turnoId.Valor}"
		);

		//_ = RefreshCache();
		return result;
	}






}
