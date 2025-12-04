CREATE PROCEDURE dbo.sp_DeleteHorarioWhereId
	@Id INT
AS
BEGIN
	DELETE
	FROM dbo.Horario
	WHERE [Id] = @Id;
END