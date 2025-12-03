CREATE PROCEDURE dbo.sp_SelectUsuarioWhereNombre
    @NombreUsuario NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
    FROM dbo.Usuario
    WHERE NombreUsuario = @NombreUsuario;
END