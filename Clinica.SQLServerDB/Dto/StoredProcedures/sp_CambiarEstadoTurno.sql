CREATE PROCEDURE [dbo].[sp_CambiarEstadoTurno]
    @TurnoId INT,
    @NuevoEstado TINYINT,
    @Comentario NVARCHAR(280) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Turno
    SET
        OutcomeEstado = @NuevoEstado,
        OutcomeFecha = GETDATE(),
        OutcomeComentario = @Comentario
    WHERE Id = @TurnoId;
END;
GO