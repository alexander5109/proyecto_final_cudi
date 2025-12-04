CREATE PROCEDURE dbo.sp_InsertTurnoReturnId
    @FechaDeCreacion        DATETIME2(1),
    @PacienteId             INT,
    @MedicoId               INT,
    @EspecialidadCodigo     INT,
    @FechaHoraAsignadaDesde DATETIME2(0),
    @FechaHoraAsignadaHasta DATETIME2(0),
    @OutcomeEstado          TINYINT,
    @OutcomeFecha           DATETIME2(1),
    @OutcomeComentario      NVARCHAR(280)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validaciones básicas recomendadas
    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE Id = @PacienteId)
    BEGIN
        RAISERROR('El PacienteId especificado no existe.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Medico WHERE Id = @MedicoId)
    BEGIN
        RAISERROR('El MedicoId especificado no existe.', 16, 1);
        RETURN;
    END

    IF (@FechaHoraAsignadaDesde >= @FechaHoraAsignadaHasta)
    BEGIN
        RAISERROR('La fecha/hora asignada desde debe ser menor que hasta.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Turno (
        FechaDeCreacion,
        PacienteId,
        MedicoId,
        EspecialidadCodigo,
        FechaHoraAsignadaDesde,
        FechaHoraAsignadaHasta,
        OutcomeEstado,
        OutcomeFecha,
        OutcomeComentario
    )
    VALUES (
        @FechaDeCreacion,
        @PacienteId,
        @MedicoId,
        @EspecialidadCodigo,
        @FechaHoraAsignadaDesde,
        @FechaHoraAsignadaHasta,
        @OutcomeEstado,
        @OutcomeFecha,
        @OutcomeComentario
    );

    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO
