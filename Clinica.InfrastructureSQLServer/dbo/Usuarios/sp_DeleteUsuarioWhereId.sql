CREATE PROCEDURE sp_DeleteUsuarioWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Usuario
    WHERE Id = @Id;

    -- Devuelvo @@ROWCOUNT para saber si borró algo
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
