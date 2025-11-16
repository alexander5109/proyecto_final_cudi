CREATE PROCEDURE [dbo].[sp_CreateMedicoWithHorarios]
    @Name NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Dni NVARCHAR(20),
    @Provincia NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @Especialidad NVARCHAR(100),
    @Telefono NVARCHAR(50),
    @Guardia BIT,
    @FechaIngreso DATE,
    @SueldoMinimoGarantizado DECIMAL(18,2),
    @Horarios dbo.HorarioMedicoTableType READONLY   -- <--- tabla de horarios
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        ---------------------------------------------------
        -- 1. INSERT DEL MÉDICO
        ---------------------------------------------------
        INSERT INTO [dbo].[Medico]
            ([Name], [LastName], [Dni], [Provincia], [Domicilio], [Localidad],
             [Especialidad], [Telefono], [Guardia], [FechaIngreso], [SueldoMinimoGarantizado])
        VALUES
            (@Name, @LastName, @Dni, @Provincia, @Domicilio, @Localidad,
             @Especialidad, @Telefono, @Guardia, @FechaIngreso, @SueldoMinimoGarantizado);

        DECLARE @NuevoMedicoId INT = SCOPE_IDENTITY();

        ---------------------------------------------------
        -- 2. INSERT DE C/U DE LOS HORARIOS
        ---------------------------------------------------
        INSERT INTO [dbo].[HorarioMedico]
            (MedicoId, DiaSemana, HoraDesde, HoraHasta)
        SELECT 
            @NuevoMedicoId,
            H.DiaSemana,
            H.HoraDesde,
            H.HoraHasta
        FROM @Horarios H;

        ---------------------------------------------------
        -- 3. DEVOLVER ID CREADO
        ---------------------------------------------------
        COMMIT TRANSACTION;

        SELECT @NuevoMedicoId AS NuevoId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO
