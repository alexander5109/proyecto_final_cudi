CREATE PROCEDURE dbo.sp_SelectAtencionesWhereTurnoId
    @TurnoId INT
AS
BEGIN
    SELECT *
    FROM dbo.Atencion
    WHERE TurnoId = @TurnoId;
END;
