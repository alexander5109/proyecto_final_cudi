CREATE PROCEDURE sp_InsertUsuarioReturnId
    @NombreUsuario NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @EnumRole TINYINT
AS
BEGIN
    INSERT INTO Usuario (NombreUsuario, PasswordHash, EnumRole)
    VALUES (@NombreUsuario, @PasswordHash, @EnumRole);

    SELECT SCOPE_IDENTITY() AS NewId;
END
