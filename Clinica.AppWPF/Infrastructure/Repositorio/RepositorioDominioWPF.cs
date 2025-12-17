using Clinica.AppWPF.Infrastructure.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.ApiDtos.ServiciosPublicosDtos;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public class RepositorioDominioWPF : IRepositorioDominioWPF {
	
	
	
	async Task<List<Disponibilidad2025>> IRepositorioDominioWPF.SelectDisponibilidades(SolicitarDisponibilidadesDto solicitud) {

		return await App.Api.TryGetJsonAsync<List<Disponibilidad2025>>(
			App.Api.BuildQuery("api/ServiciosPublicos/Turnos/Disponibilidades", solicitud),
			defaultValue: []
		);
	}




}
