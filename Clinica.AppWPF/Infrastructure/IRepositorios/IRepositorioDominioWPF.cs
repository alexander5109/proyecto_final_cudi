using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.ApiDtos.ServiciosPublicosDtos;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;




public interface IRepositorioDominioWPF {
	Task<List<Disponibilidad2025>> SelectDisponibilidades(SolicitarDisponibilidadesDto solicitud);
	//Task<IReadOnlyList<TurnoDelDia>> GetTurnosConcretadosAsync(MedicoId2025 medicoId, DateOnly fecha);


}

