CREATE PROCEDURE sp_InsertUsuarioReturnId
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
	@Nombre NVARCHAR(50),
	@Apellido NVARCHAR(50),
	@Telefono CHAR(10),
	@Email VARCHAR(320),
    @EnumRole TINYINT
AS
BEGIN
    INSERT INTO Usuario (UserName, PasswordHash, Nombre, Apellido, Telefono, Email, EnumRole)
    VALUES (@UserName, @PasswordHash, @Nombre, @Apellido, @Telefono, @Email, @EnumRole);

    SELECT SCOPE_IDENTITY() AS NewId;
END
