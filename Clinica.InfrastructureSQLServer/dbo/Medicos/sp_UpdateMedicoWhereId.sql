CREATE PROCEDURE dbo.sp_UpdateMedicoWhereId
    @Id INT,
    @EspecialidadCodigo TINYINT,
    @Dni CHAR(8),
    @Nombre VARCHAR(50),
    @Apellido VARCHAR(50),
    @FechaIngreso DATETIME2(0),
    @Domicilio VARCHAR(50),
    @Localidad VARCHAR(50),
    @ProvinciaCodigo TINYINT,
    @Telefono CHAR(10),
    @Email VARCHAR(320),
    @HaceGuardias BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validación opcional: EXISTE EL MÉDICO
    IF NOT EXISTS (SELECT 1 FROM dbo.Medico WHERE Id = @Id)
    BEGIN
        RAISERROR('No existe un médico con ese Id.', 16, 1);
        RETURN;
    END

    -- Validación opcional: DNI único
    IF EXISTS (SELECT 1 FROM dbo.Medico WHERE Dni = @Dni AND Id <> @Id)
    BEGIN
        RAISERROR('El DNI especificado ya pertenece a otro médico.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Medico
    SET 
        EspecialidadCodigo = @EspecialidadCodigo,
        Dni            = @Dni,
        Nombre         = @Nombre,
        Apellido       = @Apellido,
        FechaIngreso   = @FechaIngreso,
        Domicilio      = @Domicilio,
        Localidad      = @Localidad,
        ProvinciaCodigo = @ProvinciaCodigo,
        Telefono       = @Telefono,
        Email          = @Email,
        HaceGuardias        = @HaceGuardias
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
