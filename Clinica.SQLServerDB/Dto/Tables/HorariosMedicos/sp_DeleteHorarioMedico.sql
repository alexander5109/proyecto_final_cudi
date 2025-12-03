CREATE PROCEDURE dbo.sp_DeleteHorarioMedico
	@Id INT
AS
BEGIN
	DELETE
	FROM dbo.HorarioMedico
	WHERE [Id] = @Id;
END