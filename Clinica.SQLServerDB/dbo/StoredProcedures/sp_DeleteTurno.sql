CREATE PROCEDURE [dbo].sp_DeleteTurno
	@Id INT
AS
BEGIN
	DELETE
	FROM dbo.Turno
	WHERE [Id] = @Id;
END