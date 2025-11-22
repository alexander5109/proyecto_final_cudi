CREATE PROCEDURE [dbo].[sp_CreateTurno]
    @PacienteId INT,
    @MedicoId INT,
    @EspecialidadCodigo INT,
    @Fecha DATE,
    @Hora TIME(0),
    @DuracionMinutos INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FechaHoraDesde DATETIME = DATEADD(SECOND, 0, CAST(@Fecha AS DATETIME) + CAST(@Hora AS DATETIME));
    DECLARE @FechaHoraHasta DATETIME = DATEADD(MINUTE, @DuracionMinutos, @FechaHoraDesde);

    INSERT INTO [dbo].[Turno] (
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
        GETDATE(),               -- FechaDeCreacion
        @PacienteId,
        @MedicoId,
        @EspecialidadCodigo,
        @FechaHoraDesde,
        @FechaHoraHasta,
        1,                       -- Programado
        NULL,                    -- OutcomeFecha
        NULL                     -- OutcomeComentario
    );

    SELECT SCOPE_IDENTITY() AS NuevoId;
END;
GO
