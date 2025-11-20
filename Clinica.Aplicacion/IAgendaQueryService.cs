using System.Collections.Generic;
using System.Threading;
using Clinica.Dominio.Entidades;

namespace Clinica.Aplicacion;

public interface IAgendaQueryService{
    IAsyncEnumerable<DisponibilidadSlot> StreamDisponibilidades(
        string especialidadUId, 
        int? medicoId = null, 
        int? diaValue = null, 
        int? hora = null, 
        CancellationToken cancellationToken = default
    );
}
