CREATE PROCEDURE sp_SelectUsuarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        NombreUsuario,
        PasswordHash,
        EnumRole
    FROM Usuario;
END;
GO
