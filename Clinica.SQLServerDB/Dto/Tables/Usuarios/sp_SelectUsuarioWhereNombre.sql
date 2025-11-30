CREATE PROCEDURE sp_SelectUsuarioWhereNombre
    @NombreUsuario NVARCHAR(100)
AS
BEGIN
    SELECT *
    FROM Usuario
    WHERE NombreUsuario = @NombreUsuario;
END
