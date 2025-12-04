CREATE PROCEDURE dbo.sp_UpdateHorarioWhereId
    @Id             INT,
    @MedicoId       INT,
    @DiaSemana      TINYINT,
    @HoraDesde      TIME(0),
    @HoraHasta      TIME(0),
    @VigenteDesde   DATE,
    @VigenteHasta   DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que el horario exista
    IF NOT EXISTS (SELECT 1 FROM dbo.Horario WHERE Id = @Id)
    BEGIN
        RAISERROR('No existe un horario con ese Id.', 16, 1);
        RETURN;
    END

    -- Verificar que el médico exista (opcional pero recomendado)
    IF NOT EXISTS (SELECT 1 FROM Medico WHERE Id = @MedicoId)
    BEGIN
        RAISERROR('El médico especificado no existe.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Horario
    SET
        MedicoId     = @MedicoId,
        DiaSemana    = @DiaSemana,
        HoraDesde    = @HoraDesde,
        HoraHasta    = @HoraHasta,
        VigenteDesde = @VigenteDesde,
        VigenteHasta = @VigenteHasta
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
