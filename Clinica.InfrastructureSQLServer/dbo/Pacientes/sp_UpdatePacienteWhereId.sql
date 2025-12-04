CREATE PROCEDURE dbo.sp_UpdatePacienteWhereId
    @Id INT,
    @Dni CHAR(8),
    @Nombre VARCHAR(50),
    @Apellido VARCHAR(50),
    @FechaIngreso DATETIME2(0),
    @Email VARCHAR(320),
    @Telefono CHAR(10),
    @FechaNacimiento DATE,
    @Domicilio VARCHAR(50),
    @Localidad VARCHAR(50),
    @ProvinciaCodigo TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar existencia del paciente
    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE Id = @Id)
    BEGIN
        RAISERROR('No existe un paciente con ese Id.', 16, 1);
        RETURN;
    END

    -- Validar que el DNI sea único (excepto el mismo paciente)
    IF EXISTS (SELECT 1 FROM dbo.Paciente WHERE Dni = @Dni AND Id <> @Id)
    BEGIN
        RAISERROR('El DNI especificado ya pertenece a otro paciente.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Paciente
    SET 
        Dni             = @Dni,
        Nombre          = @Nombre,
        Apellido        = @Apellido,
        FechaIngreso    = @FechaIngreso,
        Email           = @Email,
        Telefono        = @Telefono,
        FechaNacimiento = @FechaNacimiento,
        Domicilio       = @Domicilio,
        Localidad       = @Localidad,
        ProvinciaCodigo = @ProvinciaCodigo
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
