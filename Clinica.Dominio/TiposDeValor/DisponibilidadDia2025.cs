using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;

public readonly record struct DisponibilidadDia2025(
    DateTime Fecha,
    IReadOnlyList<FranjaDisponible2025> Franjas
);
