CREATE PROCEDURE [dbo].[sp_SelectTurnosWhereMedicoIdBetweenFechas]
    @MedicoId INT,
    @FechaDesde DATETIME,
    @FechaHasta DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Turno
    WHERE MedicoId = @MedicoId
      AND FechaHoraAsignadaDesde >= @FechaDesde
      AND FechaHoraAsignadaDesde <  @FechaHasta
      AND OutcomeEstado = 0  -- 0 = Programado (ajustar si tu enum cambia)
END
GO
