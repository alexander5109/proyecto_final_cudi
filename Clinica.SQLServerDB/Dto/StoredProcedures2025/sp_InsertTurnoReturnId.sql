CREATE PROCEDURE sp_InsertTurnoReturnId
(
    @FechaDeCreacion DATETIME2(1),
    @PacienteId INT,
    @MedicoId INT,
    @EspecialidadCodigo INT,
    @FechaHoraAsignadaDesde DATETIME2(0),
    @FechaHoraAsignadaHasta DATETIME2(0),
    @NewId INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Turno (
        FechaDeCreacion,
        PacienteId,
        MedicoId,
        EspecialidadCodigo,
        FechaHoraAsignadaDesde,
        FechaHoraAsignadaHasta
    )
    VALUES (
        @FechaDeCreacion,
        @PacienteId,
        @MedicoId,
        @EspecialidadCodigo,
        @FechaHoraAsignadaDesde,
        @FechaHoraAsignadaHasta
    );

    SET @NewId = SCOPE_IDENTITY();
END;
GO
