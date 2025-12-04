CREATE PROCEDURE dbo.sp_DeleteMedicoWithHorariosWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -----------------------------------------
        -- 1. ELIMINAR HORARIOS DEL MÉDICO
        -----------------------------------------
        DELETE FROM dbo.Horario
        WHERE MedicoId = @Id;

        -----------------------------------------
        -- 2. ELIMINAR EL MÉDICO
        -----------------------------------------
        DELETE FROM dbo.Medico
        WHERE Id = @Id;

        -----------------------------------------
        -- 3. FIN
        -----------------------------------------
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
