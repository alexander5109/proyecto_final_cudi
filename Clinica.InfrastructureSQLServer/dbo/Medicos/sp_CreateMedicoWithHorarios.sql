CREATE PROCEDURE [dbo].[sp_CreateMedicoWithHorarios]
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ProvinciaCodigo NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @EspecialidadCodigoInterno INT,
    @Telefono NVARCHAR(50),
    @Guardia BIT,
    @FechaIngreso DATE,
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
            ([Nombre], [Apellido], [Dni], [ProvinciaCodigo], [Domicilio], [Localidad],
             EspecialidadCodigoInterno, [Telefono], [Guardia], [FechaIngreso])
        VALUES
            (@Nombre, @Apellido, @Dni, @ProvinciaCodigo, @Domicilio, @Localidad,
             @EspecialidadCodigoInterno, @Telefono, @Guardia, @FechaIngreso);

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
