CREATE PROCEDURE [dbo].sp_DeletePaciente
	@Id INT
AS
BEGIN
	DELETE
	FROM dbo.Paciente
	WHERE [Id] = @Id;
END