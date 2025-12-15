CREATE PROCEDURE [dbo].sp_UpdateMedicoWithHorariosWhereId
    @Id INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ProvinciaCodigo NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @EspecialidadCodigo INT,
    @Telefono NVARCHAR(50),
    @HaceGuardias BIT,
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
            EspecialidadCodigo = @EspecialidadCodigo,
            [Telefono] = @Telefono,
            [HaceGuardias] = @HaceGuardias,
            [FechaIngreso] = @FechaIngreso
        WHERE [Id] = @Id;


        -- 2) Eliminar horarios anteriores
        DELETE FROM dbo.Horario
        WHERE MedicoId = @Id;


        -- 3) Insertar horarios nuevos
        INSERT INTO dbo.Horario (
            MedicoId,
            DiaSemana,
            HoraDesde,
            HoraHasta,
            VigenteDesde,
            VigenteHasta
        )
        SELECT
            @Id,
            DiaSemana,
            HoraDesde,
            HoraHasta,
            VigenteDesde,
            VigenteHasta
        FROM @Horarios;


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH

END;
GO
