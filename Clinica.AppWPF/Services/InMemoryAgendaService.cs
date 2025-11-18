using System;
using System.Collections.Generic;
using System.Linq;
using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.Servicios;

namespace Clinica.AppWPF.Services;

public class InMemoryAgendaService : IAgendaService
{
    private readonly List<EspecialidadDto> _especialidades = new() {
        new EspecialidadDto("esp-gastro","Gastroenterólogo"),
        new EspecialidadDto("esp-psico","Psicólogo"),
        new EspecialidadDto("esp-derma","Dermatólogo")
    };

    private readonly List<MedicoSimpleDto> _medicos = new() {
        new MedicoSimpleDto(1,"Ana Perez (Gastro)"),
        new MedicoSimpleDto(2,"Luis Gomez (Gastro)"),
        new MedicoSimpleDto(3,"Marta Lopez (Psico)"),
        new MedicoSimpleDto(4,"Carla Ruiz (Derma)")
    };

    public IEnumerable<EspecialidadDto> GetEspecialidades() => _especialidades;

    public IEnumerable<MedicoSimpleDto> GetMedicosByEspecialidad(string especialidadUId)
    {
        if (string.IsNullOrEmpty(especialidadUId)) return Enumerable.Empty<MedicoSimpleDto>();
        if (especialidadUId == "esp-gastro") return _medicos.Where(m => m.Displayear.Contains("Gastro"));
        if (especialidadUId == "esp-psico") return _medicos.Where(m => m.Displayear.Contains("Psico"));
        if (especialidadUId == "esp-derma") return _medicos.Where(m => m.Displayear.Contains("Derma"));
        return Enumerable.Empty<MedicoSimpleDto>();
    }

    public IEnumerable<DiaSemanaDto> GetDiasSemana() => new[] {
        new DiaSemanaDto(0, "Domingo"),
        new DiaSemanaDto(1, "Lunes"),
        new DiaSemanaDto(2, "Martes"),
        new DiaSemanaDto(3, "Miércoles"),
        new DiaSemanaDto(4, "Jueves"),
        new DiaSemanaDto(5, "Viernes"),
        new DiaSemanaDto(6, "Sábado")
    };

    public IEnumerable<int> GetHoras() => Enumerable.Range(8, 12);

    public IEnumerable<DisponibilidadDto> GetDisponibilidades(string especialidadUId, int? medicoId, int? diaValue, int? hora)
    {
        var hoy = DateTime.Today;
        var list = new List<DisponibilidadDto>();
        var med = medicoId != null ? _medicos.FirstOrDefault(m => m.Id == medicoId) : null;
        for (int i = 0; i < 7; i++) {
            var fecha = hoy.AddDays(i);
            if (diaValue != null && (int)fecha.DayOfWeek != diaValue) continue;
            for (int h = 9; h <= 16; h++) {
                if (hora != null && h != hora) continue;
                var medicoName = med?.Displayear ?? _medicos.First().Displayear;
                list.Add(new DisponibilidadDto(fecha, TimeSpan.FromHours(h).ToString(@"hh\:mm"), medicoName));
            }
        }
        return list;
    }
}
