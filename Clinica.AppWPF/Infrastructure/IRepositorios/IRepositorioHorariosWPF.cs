using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;


public interface IRepositorioHorariosWPF {
	//Task<List<HorarioDbModel>> SelectHorarios();
	Task<IReadOnlyList<HorarioDbModel>?> SelectHorariosWhereMedicoId(MedicoId id);
	Task<IReadOnlyList<DayOfWeek>?> SelectDiasDeAtencionWhereMedicoId(MedicoId id);
	Task<ResultWpf<UnitWpf>> UpdateHorariosWhereMedicoId(HorariosMedicos2026Agg agregado);
}


