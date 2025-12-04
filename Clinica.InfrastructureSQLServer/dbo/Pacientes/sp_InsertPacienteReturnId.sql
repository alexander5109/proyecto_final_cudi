CREATE PROCEDURE dbo.sp_InsertPacienteReturnId
    @Dni CHAR(8),
    @Nombre VARCHAR(50),
    @Apellido VARCHAR(50),
    @FechaIngreso DATETIME2(0),
    @Domicilio VARCHAR(50),
    @Localidad VARCHAR(50),
    @ProvinciaCodigo TINYINT,
    @Telefono CHAR(10),
    @Email VARCHAR(320),
    @FechaNacimiento DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Paciente WHERE Dni = @Dni)
    BEGIN
        RAISERROR('Ya existe un paciente con ese DNI.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Paciente (
        Dni, Nombre, Apellido, FechaIngreso,
        Domicilio, Localidad, ProvinciaCodigo,
        Telefono, Email, FechaNacimiento
    )
    VALUES (
        @Dni, @Nombre, @Apellido, @FechaIngreso,
        @Domicilio, @Localidad, @ProvinciaCodigo,
        @Telefono, @Email, @FechaNacimiento
    );

    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO
