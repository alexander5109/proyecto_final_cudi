using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Repositorios;
using Clinica.Dominio.TiposDeValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinica.Dominio.Servicios;

public static class AgendaEspecialidadService
{
    // Devuelve disponibilidad por día en un rango (inclusive) para una especialidad.
    public static Result<DisponibilidadEspecialidad2025> CalcularDisponibilidad(
        IRepositorioMedicos repoMedicos,
        IRepositorioTurnos repoTurnos,
        MedicoEspecialidad2025 especialidad,
        DateTime desde,
        DateTime hasta
    )
    {
        if (desde.Date > hasta.Date)
            return new Result<DisponibilidadEspecialidad2025>.Error("Rango inválido: 'desde' es posterior a 'hasta'.");

        Result<IReadOnlyList<Medico2025>> medicosR = repoMedicos.ObtenerMedicosPorEspecialidad(especialidad.Titulo);
        if (medicosR is Result<IReadOnlyList<Medico2025>>.Error medErr)
            return new Result<DisponibilidadEspecialidad2025>.Error(medErr.Mensaje);

        IReadOnlyList<Medico2025> medicos = ((Result<IReadOnlyList<Medico2025>>.Ok)medicosR).Valor;

        // Obtener turnos de la especialidad en el rango
        Result<IReadOnlyList<Turno2025>> turnosR = repoTurnos.ObtenerTurnosPorEspecialidad(especialidad.Titulo, desde, hasta);
        if (turnosR is Result<IReadOnlyList<Turno2025>>.Error turErr)
            return new Result<DisponibilidadEspecialidad2025>.Error(turErr.Mensaje);

        IReadOnlyList<Turno2025> turnos = ((Result<IReadOnlyList<Turno2025>>.Ok)turnosR).Valor;

        List<DisponibilidadDia2025> dias = new List<DisponibilidadDia2025>();

        for (var date = desde.Date; date <= hasta.Date; date = date.AddDays(1))
        {
            var franjas = new List<FranjaDisponible2025>();
            var diaSemana = date.DayOfWeek;

            // Para cada médico, mirar horarios de ese día y generar slots disponibles
            foreach (var medico in medicos)
            {
                var horarios = medico.ListaHorarios.Valores
                    .Where(h => h.DiaSemana.Valor == diaSemana)
                    .ToList();

                foreach (var horario in horarios)
                {
                    var inicio = horario.Desde.Valor; // TimeOnly
                    var fin = horario.Hasta.Valor;
                    var duracion = TimeSpan.FromMinutes(especialidad.DuracionConsultaMinutos);

                    // Generar slots en este horario sin tomar en cuenta aún turnos existentes
                    var cursor = inicio;
                    while (cursor.Add(duracion) <= fin)
                    {
                        var slotStart = DateTime.SpecifyKind(date.Add(cursor.ToTimeSpan()), DateTimeKind.Local);
                        var slotEnd = slotStart.Add(duracion);

                        // Contar cuántos médicos disponen para ese slot (al menos 1)
                        // Chequear turnos existentes que se solapen con el slot
                        bool slotOcupado = turnos.Any(t =>
                            t.FechaYHora < slotEnd && t.FechaYHora.Add(TimeSpan.FromMinutes(t.Especialidad.DuracionConsultaMinutos)) > slotStart
                        );

                        if (!slotOcupado)
                        {
                            // Agregar como franja de 1 turno posible en esta posición
                            franjas.Add(new FranjaDisponible2025(TimeOnly.FromTimeSpan(slotStart.TimeOfDay), TimeOnly.FromTimeSpan(slotEnd.TimeOfDay), 1));
                        }

                        cursor = cursor.Add(duracion);
                    }
                }
            }

            // Agrupar franjas contiguas y sumar turnos posibles
            var agrupadas = franjas
                .OrderBy(f => f.Desde)
                .GroupBy(f => new { f.Desde, f.Hasta })
                .Select(g => new FranjaDisponible2025(g.Key.Desde, g.Key.Hasta, g.Sum(x => x.TurnosPosibles)))
                .ToList();

            dias.Add(new DisponibilidadDia2025(date, agrupadas));
        }

        var disponibilidad = new DisponibilidadEspecialidad2025(especialidad, TimeSpan.FromMinutes(especialidad.DuracionConsultaMinutos), dias);
        return new Result<DisponibilidadEspecialidad2025>.Ok(disponibilidad);
    }
}
