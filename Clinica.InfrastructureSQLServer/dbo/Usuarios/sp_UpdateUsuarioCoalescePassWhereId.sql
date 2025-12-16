CREATE PROCEDURE dbo.sp_UpdateUsuarioCoalescePassWhereId
    @Id INT,
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255) = NULL, -- 👈 clave
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
        UserName     = @UserName,
        PasswordHash = COALESCE(@PasswordHash, PasswordHash), -- 👈 magia
        Nombre       = @Nombre,
        Apellido     = @Apellido,
        Telefono     = @Telefono,
        Email        = @Email,
        EnumRole     = @EnumRole
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
