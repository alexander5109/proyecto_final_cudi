CREATE PROCEDURE [dbo].[sp_DeletePaciente]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Paciente
    WHERE Id = @Id;
END;
GO
