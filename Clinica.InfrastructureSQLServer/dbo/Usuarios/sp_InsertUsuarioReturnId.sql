CREATE PROCEDURE sp_InsertUsuarioReturnId
    @NombreUsuario NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @EnumRole TINYINT,
    @NewId INT OUTPUT
AS
BEGIN
    INSERT INTO Usuario (NombreUsuario, PasswordHash, EnumRole)
    VALUES (@NombreUsuario, @PasswordHash, @EnumRole);

    SET @NewId = SCOPE_IDENTITY();
END
