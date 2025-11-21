CREATE PROCEDURE [dbo].[sp_CreateTurno]
    @PacienteId INT,
    @MedicoId INT,
    @Fecha DATE,
    @Hora TIME(0)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Turno]
        ([PacienteId], [MedicoId], [Fecha], [Hora])
    VALUES
        (@PacienteId, @MedicoId, @Fecha, @Hora);

    SELECT SCOPE_IDENTITY() AS NuevoId;
END;
GO
