CREATE PROCEDURE [dbo].sp_DeletePacienteWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Paciente
    WHERE Id = @Id;
END;
GO
