CREATE PROCEDURE dbo.sp_DeleteAtencionWhereId
	@Id INT
AS
BEGIN
	DELETE
	FROM dbo.Atencion
	WHERE [Id] = @Id;
END