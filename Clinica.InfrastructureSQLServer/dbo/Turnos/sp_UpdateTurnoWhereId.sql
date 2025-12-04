CREATE PROCEDURE dbo.sp_UpdateTurnoWhereId
    @Id INT,
    @FechaDeCreacion       DATETIME2(1),
    @PacienteId            INT,
    @MedicoId              INT,
    @EspecialidadCodigo    INT,
    @FechaHoraAsignadaDesde DATETIME2(0),
    @FechaHoraAsignadaHasta DATETIME2(0),
    @OutcomeEstado         TINYINT,
    @OutcomeFecha          DATETIME2(1),
    @OutcomeComentario     NVARCHAR(280)
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------
    -- VALIDACIONES RECOMENDADAS
    ----------------------------------------------------------

    -- Validar existencia del turno
    IF NOT EXISTS (SELECT 1 FROM dbo.Turno WHERE Id = @Id)
    BEGIN
        RAISERROR('No existe un turno con ese Id.', 16, 1);
        RETURN;
    END

    -- Validar existencia del paciente
    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE Id = @PacienteId)
    BEGIN
        RAISERROR('El PacienteId especificado no existe.', 16, 1);
        RETURN;
    END

    -- Validar existencia del médico
    IF NOT EXISTS (SELECT 1 FROM dbo.Medico WHERE Id = @MedicoId)
    BEGIN
        RAISERROR('El MedicoId especificado no existe.', 16, 1);
        RETURN;
    END

    -- Validación lógica opcional (la tabla NO tiene constraint)
    IF (@FechaHoraAsignadaDesde >= @FechaHoraAsignadaHasta)
    BEGIN
        RAISERROR('La fecha/hora asignada desde debe ser menor que hasta.', 16, 1);
        RETURN;
    END


    ----------------------------------------------------------
    -- UPDATE COMPLETO
    ----------------------------------------------------------

    UPDATE dbo.Turno
    SET
        FechaDeCreacion        = @FechaDeCreacion,
        PacienteId             = @PacienteId,
        MedicoId               = @MedicoId,
        EspecialidadCodigo     = @EspecialidadCodigo,
        FechaHoraAsignadaDesde = @FechaHoraAsignadaDesde,
        FechaHoraAsignadaHasta = @FechaHoraAsignadaHasta,
        OutcomeEstado          = @OutcomeEstado,
        OutcomeFecha           = @OutcomeFecha,
        OutcomeComentario      = @OutcomeComentario
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
