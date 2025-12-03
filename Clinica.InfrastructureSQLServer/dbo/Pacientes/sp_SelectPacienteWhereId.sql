CREATE PROCEDURE dbo.sp_SelectPacienteWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
	FROM dbo.Paciente
    WHERE Id = @Id;
END;
GO
