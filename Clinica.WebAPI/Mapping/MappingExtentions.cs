using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.WebAPI.DTOs;

namespace Clinica.WebAPI.Mapping;

public static class MappingExtensions {
	public static DisponibilidadDTO ToDTO(this DisponibilidadEspecialidad2025 d) =>
		new(
			d.MedicoId.Valor,
			d.FechaHoraDesde,
			d.FechaHoraHasta,
			d.Especialidad.CodigoInterno.Valor
		);
}
