CREATE TYPE dbo.HorarioMedicoTableType AS TABLE(
    DiaSemana      TINYINT,
    HoraDesde      TIME(0),
    HoraHasta      TIME(0),
    VigenteDesde   DATE,
    VigenteHasta   DATE NULL
);
GO