CREATE PROCEDURE dbo.sp_SelectUsuarioWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
    FROM dbo.Usuario
    WHERE Id = @Id;
END
