CREATE PROCEDURE dbo.sp_InsertAtencionReturnId
    @TurnoId INT,
    @PacienteId INT,
    @MedicoId INT,
    @Fecha DATETIME,
    @Observaciones NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO dbo.Atencion (TurnoId, PacienteId, MedicoId, Fecha, Observaciones)
    VALUES (@TurnoId, @PacienteId, @MedicoId, @Fecha, @Observaciones);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END;
