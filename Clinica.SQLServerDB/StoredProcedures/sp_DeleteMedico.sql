CREATE PROCEDURE [dbo].sp_DeleteMedico
	@Id INT
AS
BEGIN
	DELETE
	FROM dbo.Medico
	WHERE [Id] = @Id;
END