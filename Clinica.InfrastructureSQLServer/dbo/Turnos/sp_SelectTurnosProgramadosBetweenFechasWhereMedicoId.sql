CREATE PROCEDURE dbo.sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId
    @MedicoId INT,
    @FechaDesde DATETIME,
    @FechaHasta DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, 
        MedicoId, 
        EspecialidadCodigo, 
        FechaHoraAsignadaDesde, 
        FechaHoraAsignadaHasta, 
        OutcomeEstado
    FROM dbo.Turno
    WHERE MedicoId = @MedicoId
      AND OutcomeEstado = 1 -- Programado
      AND FechaHoraAsignadaDesde < @FechaHasta
      AND FechaHoraAsignadaHasta > @FechaDesde
    ORDER BY FechaHoraAsignadaDesde ASC;
END
GO
