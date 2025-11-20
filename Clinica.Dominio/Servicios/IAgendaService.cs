using System.Collections.Generic;

namespace Clinica.Dominio.Servicios;

public interface IAgendaService
{
    IEnumerable<Clinica.Dominio.Dtos.EspecialidadDto> GetEspecialidades();
    IEnumerable<Clinica.Dominio.Dtos.MedicoSimpleDto> GetMedicosByEspecialidad(string especialidadUId);
    IEnumerable<Clinica.Dominio.Dtos.DiaSemanaDto> GetDiasSemana();
    IEnumerable<int> GetHoras();
    IEnumerable<Clinica.Dominio.Dtos.DisponibilidadDto> GetDisponibilidades(string especialidadUId, int? medicoId, int? diaValue, int? hora);
}
