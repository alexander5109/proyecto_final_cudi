using System.Net.Http.Json;
using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using Clinica.Shared.DBModels;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioAtencionesWPF : IRepositorioAtencionesWPF {

	async Task<List<AtencionDbModel>> IRepositorioAtencionesWPF.SelectAtenciones() {
		return await App.Api.TryGetJsonOrNullAsync<List<AtencionDbModel>>(
			"api/Atenciones"
		) ?? new List<AtencionDbModel>();
	}

	async Task<List<AtencionDbModel>> IRepositorioAtencionesWPF.SelectAtencionesWherePacienteId(PacienteId2025 id) {
		return await App.Api.TryGetJsonOrNullAsync<List<AtencionDbModel>>(
			$"api/Atenciones/paciente/{id.Valor}"
		) ?? new List<AtencionDbModel>();
	}

	async Task<ResultWpf<UnitWpf>> IRepositorioAtencionesWPF.AgendarAtencionConDiagnostico(AtencionDto atencionDto) {

		ResultWpf<UnitWpf> result = await App.Api.TryApiCallAsync(
			httpCall: () => App.Api.Cliente.PostAsJsonAsync("api/Atenciones", atencionDto),
			onOk: async response => UnitWpf.Valor,
			errorTitle: $"Error agendando atención para el turno {atencionDto.TurnoId}"
		);

		return result;
	}

	async Task<List<TurnoDbModel>> IRepositorioAtencionesWPF.SelectTurnosWhereMedicoId(MedicoId2025 id) {
		return await App.Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/medicos/{id.Valor}/turnos"
		) ?? new List<TurnoDbModel>();
	}

	async Task<List<TurnoDbModel>> IRepositorioAtencionesWPF.SelectTurnosWhereMedicoIdDeLaFecha(MedicoId2025 id, DateOnly fecha) {
		return await App.Api.TryGetJsonOrNullAsync<List<TurnoDbModel>>(
			$"api/medicos/{id.Valor}/turnos/{fecha:yyyy-MM-dd}"
		) ?? new List<TurnoDbModel>();
	}
}
