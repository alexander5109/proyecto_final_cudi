CREATE PROCEDURE dbo.sp_SelectHorariosVigentesBetweenFechasWhereMedicoId
    @MedicoId INT,
    @FechaDesde DATE,
    @FechaHasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        -- Id,
        MedicoId,
        DiaSemana,
        HoraDesde,
        HoraHasta
        -- VigenteDesde,
        -- VigenteHasta
    FROM dbo.Horario
    WHERE MedicoId = @MedicoId
      AND VigenteDesde <= @FechaHasta
      AND (VigenteHasta IS NULL OR VigenteHasta >= @FechaDesde)
END
GO
