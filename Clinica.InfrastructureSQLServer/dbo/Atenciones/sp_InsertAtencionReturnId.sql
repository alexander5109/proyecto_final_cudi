CREATE PROCEDURE dbo.sp_InsertAtencionReturnId
    @Id INT,
    @TurnoId INT,
    @PacienteId INT,
    @MedicoId INT,
    @Observaciones NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO dbo.Atencion (Id, TurnoId, PacienteId, MedicoId, Observaciones)
    VALUES (@Id, @TurnoId, @PacienteId, @MedicoId, @Observaciones);

    SELECT SCOPE_IDENTITY() AS NewId;
END;
