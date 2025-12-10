CREATE PROCEDURE dbo.sp_UpdateUsuarioWhereId
    @Id INT,
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
	@Nombre NVARCHAR(50),
	@Apellido NVARCHAR(50),
	@Telefono CHAR(10),
	@Email VARCHAR(320),
    @EnumRole TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Usuario
    SET
        UserName	= @UserName,
        PasswordHash	= @PasswordHash,
        Nombre			= @Nombre,
        Apellido		= @Apellido,
        Telefono		= @Telefono,
        Email			= @Email,
        EnumRole		= @EnumRole
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
