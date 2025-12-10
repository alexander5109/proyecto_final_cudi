CREATE PROCEDURE dbo.sp_SelectUsuarioWhereNombre
    @UserName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
    FROM dbo.Usuario
    WHERE UserName = @UserName;
END