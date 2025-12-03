CREATE PROCEDURE dbo.sp_SelectTurnosWhereMedicoId
    @MedicoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Turno
    WHERE MedicoId = @MedicoId;
END;
GO
