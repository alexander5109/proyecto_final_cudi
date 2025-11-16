using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;
using System.Collections.Generic;

namespace Clinica.Dominio.Entidades;

public readonly record struct DisponibilidadEspecialidad2025(
    EspecialidadMedica2025 Especialidad,
    TimeSpan DuracionConsulta,
    IReadOnlyList<DisponibilidadDia2025> DiasDisponibles
);
