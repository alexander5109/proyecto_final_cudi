CREATE TYPE dbo.HorarioMedicoTableType AS TABLE
(
    DiaSemana INT,
    HoraDesde TIME(7),
    HoraHasta TIME(7)
);
GO
