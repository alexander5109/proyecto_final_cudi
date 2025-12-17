using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioHorarios {
	
	


	Task<Result<Unit>> DeleteHorarioWhereId(HorarioId2025 id);
	//Task<Result<HorarioId2025>> InsertHorarioReturnId(Horario2025 instance);
	Task<Result<IEnumerable<HorarioDbModel>>> SelectHorarios();
	Task<Result<HorarioDbModel?>> SelectHorarioWhereId(HorarioId2025 id);
	//Task<Result<HorarioDbModel>> UpdateHorarioWhereId(HorarioId2025 id, Horario2025 instance);



	//----------------------- MERGING --------------------//


	//Task<Result<IEnumerable<HorarioDbModel>>> SelectHorarios();
	//Task<Result<IEnumerable<HorarioDbModel>>> SelectHorariosWhereMedicoId(MedicoId2025 medicoId);
	Task<Result<Unit>> UpsertHorariosWhereMedicoId(HorariosMedicos2026Agg aggrg);
	//Task<Result<Unit>> DeleteHorariosWhereMedicoId(MedicoId2025 medicoId);
}
