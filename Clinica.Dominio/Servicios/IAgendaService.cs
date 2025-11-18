using System.Collections.Generic;

namespace Clinica.Dominio.Servicios;

public interface IAgendaService
{
    IEnumerable<EspecialidadDto> GetEspecialidades();
    IEnumerable<MedicoSimpleDto> GetMedicosByEspecialidad(string especialidadUId);
    IEnumerable<DiaSemanaDto> GetDiasSemana();
    IEnumerable<int> GetHoras();
    IEnumerable<DisponibilidadDto> GetDisponibilidades(string especialidadUId, int? medicoId, int? diaValue, int? hora);
}
