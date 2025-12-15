CREATE TYPE dbo.HorarioFranjaUDTT AS TABLE (
    DiaSemana     TINYINT     NOT NULL,
    HoraDesde     TIME(0)     NOT NULL,
    HoraHasta     TIME(0)     NOT NULL,
    VigenteDesde  DATE        NOT NULL,
    VigenteHasta  DATE        NULL
);
