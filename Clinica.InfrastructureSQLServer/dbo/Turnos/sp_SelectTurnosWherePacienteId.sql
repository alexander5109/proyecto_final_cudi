CREATE PROCEDURE dbo.sp_SelectTurnosWherePacienteId
    @PacienteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Turno
    WHERE PacienteId = @PacienteId;
END;
GO
