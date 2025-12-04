CREATE PROCEDURE dbo.sp_SelectHorarioWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
	FROM dbo.Horario
    WHERE Id = @Id;
END;
GO
