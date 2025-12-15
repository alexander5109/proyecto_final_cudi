CREATE PROCEDURE dbo.sp_UpsertHorariosWhereMedicoId
    @MedicoId INT,
    @Franjas dbo.HorarioFranjaUDTT READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Borrar horarios existentes
        DELETE FROM dbo.Horario
        WHERE MedicoId = @MedicoId;

        -- 2. Insertar nuevas franjas
        INSERT INTO dbo.Horario (
            MedicoId,
            DiaSemana,
            HoraDesde,
            HoraHasta,
            VigenteDesde,
            VigenteHasta
        )
        SELECT
            @MedicoId,
            f.DiaSemana,
            f.HoraDesde,
            f.HoraHasta,
            f.VigenteDesde,
            f.VigenteHasta
        FROM @Franjas f;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        THROW;
    END CATCH
END
GO
