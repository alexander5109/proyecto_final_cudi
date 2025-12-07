CREATE PROCEDURE dbo.sp_InsertMedicoWithHorariosReturnId
    @Dni                     CHAR(8),
    @Nombre                 VARCHAR(50),
    @Apellido               VARCHAR(50),
    @FechaIngreso           DATETIME2(0),
    @Domicilio              VARCHAR(50),
    @Localidad              VARCHAR(50),
    @ProvinciaCodigo        TINYINT,
    @Telefono               CHAR(10),
    @Email                  VARCHAR(320),
    @EspecialidadCodigo TINYINT,
    @Guardia                BIT,
    @Horarios               dbo.HorarioMedicoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        ------------------------------------
        -- VALIDAR QUE EL DNI NO EXISTA
        ------------------------------------
        IF EXISTS (SELECT 1 FROM dbo.Medico WHERE Dni = @Dni)
        BEGIN
            RAISERROR('Ya existe un médico con ese DNI.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        ------------------------------------
        -- INSERT MÉDICO
        ------------------------------------
        INSERT INTO dbo.Medico (
            EspecialidadCodigo,
            Dni,
            Nombre,
            Apellido,
            FechaIngreso,
            Domicilio,
            Localidad,
            ProvinciaCodigo,
            Telefono,
            Email,
            Guardia
        )
        VALUES (
            @EspecialidadCodigo,
            @Dni,
            @Nombre,
            @Apellido,
            @FechaIngreso,
            @Domicilio,
            @Localidad,
            @ProvinciaCodigo,
            @Telefono,
            @Email,
            @Guardia
        );

        DECLARE @NuevoMedicoId INT = SCOPE_IDENTITY();

        ------------------------------------
        -- INSERT HORARIOS
        ------------------------------------
        INSERT INTO dbo.Horario (
            MedicoId,
            DiaSemana,
            HoraDesde,
            HoraHasta,
            VigenteDesde,
            VigenteHasta
        )
        SELECT 
            @NuevoMedicoId,
            H.DiaSemana,
            H.HoraDesde,
            H.HoraHasta,
            H.VigenteDesde,
            H.VigenteHasta
        FROM @Horarios H;

        ------------------------------------
        -- FIN
        ------------------------------------
        COMMIT TRANSACTION;

        SELECT @NuevoMedicoId AS NuevoId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO
