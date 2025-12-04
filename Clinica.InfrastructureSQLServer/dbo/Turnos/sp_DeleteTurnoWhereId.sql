CREATE PROCEDURE dbo.sp_DeleteTurnoWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Turno
    WHERE Id = @Id;

    -- Devuelvo @@ROWCOUNT para saber si borró algo
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
