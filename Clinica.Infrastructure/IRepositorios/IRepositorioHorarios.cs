using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioHorarios {
	
	


	Task<Result<Unit>> DeleteHorarioWhereId(HorarioId id);
	//Task<Result<HorarioId>> InsertHorarioReturnId(Horario2025 instance);
	Task<Result<IEnumerable<HorarioDbModel>>> SelectHorarios();
	Task<Result<HorarioDbModel?>> SelectHorarioWhereId(HorarioId id);
	//Task<Result<HorarioDbModel>> UpdateHorarioWhereId(HorarioId id, Horario2025 instance);



	//----------------------- MERGING --------------------//


	//Task<Result<IEnumerable<HorarioDbModel>>> SelectHorarios();
	//Task<Result<IEnumerable<HorarioDbModel>>> SelectHorariosWhereMedicoId(MedicoId medicoId);
	Task<Result<Unit>> UpsertHorariosWhereMedicoId(HorariosMedicos2026Agg aggrg);
	//Task<Result<Unit>> DeleteHorariosWhereMedicoId(MedicoId medicoId);
}
