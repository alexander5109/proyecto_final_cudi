CREATE PROCEDURE dbo.sp_SelectTurnoWhereId
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
    SELECT TOP 1 *
	FROM dbo.[Turno]
	WHERE Id = @Id;
END;
GO