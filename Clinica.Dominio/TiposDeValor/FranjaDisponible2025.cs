namespace Clinica.Dominio.TiposDeValor;

public readonly record struct FranjaDisponible2025(
    TimeOnly Desde,
    TimeOnly Hasta,
    int TurnosPosibles
);
