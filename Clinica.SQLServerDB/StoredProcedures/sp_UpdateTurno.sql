CREATE PROCEDURE [dbo].[sp_UpdateTurno]
    @Id INT,
    @PacienteId INT,
    @MedicoId INT,
    @Fecha DATE,
    @Hora TIME(0)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Turno]
    SET 
        [PacienteId] = @PacienteId,
        [MedicoId] = @MedicoId,
        [Fecha] = @Fecha,
        [Hora] = @Hora
    WHERE [Id] = @Id;
END;
GO
