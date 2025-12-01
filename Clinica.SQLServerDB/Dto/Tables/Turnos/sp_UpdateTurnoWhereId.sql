CREATE PROCEDURE sp_UpdateTurnoWhereId
(
    @TurnoId INT,
    @OutcomeEstado TINYINT,
    @OutcomeFecha DATETIME2(1),
    @OutcomeComentario NVARCHAR(280)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Turno
    SET 
        OutcomeEstado = @OutcomeEstado,
        OutcomeFecha = @OutcomeFecha,
        OutcomeComentario = @OutcomeComentario
    WHERE Id = @TurnoId;

    -- Verificar que se haya actualizado
    IF (@@ROWCOUNT = 0)
    BEGIN
        RAISERROR('No existe un turno con ese Id.', 16, 1);
        RETURN;
    END
END;
GO
