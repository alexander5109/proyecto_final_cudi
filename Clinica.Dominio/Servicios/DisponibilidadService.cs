using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.Servicios;

// Application-level adapter that uses domain repositories to stream availability slots
public class DisponibilidadService : IAgendaQueryService {
	private readonly IRepositorioMedicos _repoMedicos;
	private readonly IRepositorioTurnos _repoTurnos;

	public DisponibilidadService(IRepositorioMedicos repoMedicos, IRepositorioTurnos repoTurnos) {
		_repoMedicos = repoMedicos;
		_repoTurnos = repoTurnos;
	}

	public async IAsyncEnumerable<DisponibilidadSlot> StreamDisponibilidades(
		string especialidadUId, 
		int? medicoId = null, 
		int? diaValue = null, 
		int? hora = null, 
		[System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default
	) {
		// Simple generator: yield next 100 slots for demo. Real impl should consult horarios and turnos via repos.
		var today = DateTime.Today;
		int count = 0;
		for (int d = 0; d < 30 && count < 100; d++) {
			cancellationToken.ThrowIfCancellationRequested();
			var fecha = today.AddDays(d);
			if (diaValue.HasValue && (int)fecha.DayOfWeek != diaValue.Value) continue;

			for (int h = 8; h <= 19 && count < 15; h++) {
				if (hora.HasValue && hora.Value != h) continue;
				var fechaHora = fecha.AddHours(h);
				yield return new DisponibilidadSlot(fechaHora, medicoId, medicoId.HasValue ? $"Medico {medicoId}" : "Cualquier médico");
				count++;
			}
		}
		await System.Threading.Tasks.Task.CompletedTask;
	}
}
