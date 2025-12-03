CREATE PROCEDURE dbo.sp_UpdateUsuarioWhereId
    @Id INT,
    @NombreUsuario NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @EnumRole TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Usuario
    SET
        NombreUsuario = @NombreUsuario,
        PasswordHash  = @PasswordHash,
        EnumRole      = @EnumRole
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
