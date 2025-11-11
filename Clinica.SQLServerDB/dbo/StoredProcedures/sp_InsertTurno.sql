CREATE PROCEDURE dbo.sp_InsertTurno
    @PacienteId INT,
    @MedicoId INT,
    @Fecha DATE,
    @Hora TIME
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO dbo.Turno (PacienteId, MedicoId, Fecha, Hora)
        VALUES (@PacienteId, @MedicoId, @Fecha, @Hora);

        -- devolver la nueva ID generada
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS TurnoId;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
