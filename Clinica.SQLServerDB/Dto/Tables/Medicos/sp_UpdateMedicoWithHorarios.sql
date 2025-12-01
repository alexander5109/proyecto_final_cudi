CREATE PROCEDURE [dbo].[sp_UpdateMedicoWithHorarios]
    @Id INT,
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
    @Horarios dbo.HorarioMedicoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY

        -- 1) Update de los datos del médico
        UPDATE [dbo].[Medico]
        SET 
            [Nombre] = @Nombre,
            [Apellido] = @Apellido,
            [Dni] = @Dni,
            [ProvinciaCodigo] = @ProvinciaCodigo,
            [Domicilio] = @Domicilio,
            [Localidad] = @Localidad,
            EspecialidadCodigoInterno = @EspecialidadCodigoInterno,
            [Telefono] = @Telefono,
            [Guardia] = @Guardia,
            [FechaIngreso] = @FechaIngreso
        WHERE [Id] = @Id;


        -- 2) Eliminar horarios anteriores
        DELETE FROM dbo.HorarioMedico
        WHERE MedicoId = @Id;


        -- 3) Insertar horarios nuevos
        INSERT INTO dbo.HorarioMedico (MedicoId, DiaSemana, HoraDesde, HoraHasta)
        SELECT @Id, DiaSemana, HoraDesde, HoraHasta
        FROM @Horarios;


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH

END;
GO
