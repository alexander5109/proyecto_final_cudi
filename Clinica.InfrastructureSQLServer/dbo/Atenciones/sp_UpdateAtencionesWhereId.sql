CREATE PROCEDURE dbo.sp_ActualizarObservaciones
    @Id UNIQUEIDENTIFIER,
    @Observaciones NVARCHAR(MAX)
AS
BEGIN
    UPDATE dbo.Atencion
    SET Observaciones = @Observaciones
    WHERE Id = @Id;
END;
