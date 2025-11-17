CREATE PROCEDURE [dbo].[sp_UpdateMedicoWithHorarios]
    @Id INT,
    @Name NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Dni NVARCHAR(20),
    @Provincia NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @EspecialidadCodigoInterno INT,
    @Telefono NVARCHAR(50),
    @Guardia BIT,
    @FechaIngreso DATE,
    @SueldoMinimoGarantizado DECIMAL(18,2),

    @Horarios dbo.HorarioMedicoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY

        -- 1) Update de los datos del médico
        UPDATE [dbo].[Medico]
        SET 
            [Name] = @Name,
            [LastName] = @LastName,
            [Dni] = @Dni,
            [Provincia] = @Provincia,
            [Domicilio] = @Domicilio,
            [Localidad] = @Localidad,
            EspecialidadCodigoInterno = @EspecialidadCodigoInterno,
            [Telefono] = @Telefono,
            [Guardia] = @Guardia,
            [FechaIngreso] = @FechaIngreso,
            [SueldoMinimoGarantizado] = @SueldoMinimoGarantizado
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
