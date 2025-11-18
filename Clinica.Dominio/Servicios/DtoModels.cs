using System;

namespace Clinica.Dominio.Servicios;

public record EspecialidadDto(string UId, string Titulo);
public record MedicoSimpleDto(int Id, string Displayear);
public record DiaSemanaDto(int Value, string NombreDia);
public record DisponibilidadDto(DateTime Fecha, string Hora, string Medico);
