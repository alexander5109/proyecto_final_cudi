CREATE PROCEDURE sp_SelectUsuarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        UserName,
        PasswordHash,
        Nombre,
        Apellido,
        Telefono,
        Email
        EnumRole
    FROM Usuario;
END;
GO
