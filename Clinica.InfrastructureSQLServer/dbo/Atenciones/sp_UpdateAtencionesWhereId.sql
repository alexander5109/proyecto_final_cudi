CREATE PROCEDURE dbo.sp_UpdateAtencionesWhereId
    @Id INT,
    @Observaciones NVARCHAR(MAX)
AS
BEGIN
    UPDATE dbo.Atencion
    SET Observaciones = @Observaciones
    WHERE Id = @Id;
END;
