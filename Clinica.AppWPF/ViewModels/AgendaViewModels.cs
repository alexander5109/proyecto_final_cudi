using System;
using System.Collections.Generic;

namespace Clinica.AppWPF.ViewModels;

public record EspecialidadMedicaDto(string UId, string Titulo);
public record MedicoSimpleDto(int Id, string Displayear);
public record DisponibilidadDto(DateTime Fecha, string Hora, string Medico);
