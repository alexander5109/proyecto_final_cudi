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
      AND FechaHoraAsignadaDesde >= @FechaDesde
      AND FechaHoraAsignadaDesde <  @FechaHasta
      AND OutcomeEstado = 0  -- 0 = Programado (ajustar si tu enum cambia)
END
GO
