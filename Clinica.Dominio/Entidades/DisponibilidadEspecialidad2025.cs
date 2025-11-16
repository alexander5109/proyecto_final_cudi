using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;
using System.Collections.Generic;

namespace Clinica.Dominio.Entidades;

public readonly record struct FranjaDisponible2025(
    TimeOnly Desde,
    TimeOnly Hasta,
    int TurnosPosibles
);

public readonly record struct DisponibilidadDia2025(
    DateTime Fecha,
    IReadOnlyList<FranjaDisponible2025> Franjas
);

public readonly record struct DisponibilidadEspecialidad2025(
    MedicoEspecialidad2025 Especialidad,
    TimeSpan DuracionConsulta,
    IReadOnlyList<DisponibilidadDia2025> DiasDisponibles
);
