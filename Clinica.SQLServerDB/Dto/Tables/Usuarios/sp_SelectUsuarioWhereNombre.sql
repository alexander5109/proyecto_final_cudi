CREATE PROCEDURE sp_SelectUsuarioWhereNombre
    @NombreUsuario NVARCHAR(100)
AS
BEGIN
    SELECT TOP 1 *
    FROM Usuario
    WHERE NombreUsuario = @NombreUsuario;
END