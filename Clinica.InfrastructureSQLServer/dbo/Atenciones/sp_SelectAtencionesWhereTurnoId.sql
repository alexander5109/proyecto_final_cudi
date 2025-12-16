CREATE PROCEDURE dbo.sp_SelectAtencionPorTurno
    @TurnoId INT
AS
BEGIN
    SELECT *
    FROM dbo.Atencion
    WHERE TurnoId = @TurnoId;
END;
