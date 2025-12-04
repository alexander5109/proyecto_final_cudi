CREATE PROCEDURE [dbo].sp_DeleteMedicoWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Medico
    WHERE Id = @Id;
END;
GO
