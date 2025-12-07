CREATE PROCEDURE dbo.sp_InsertMedicoReturnId
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
    @Guardia BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Medico WHERE Dni = @Dni)
    BEGIN
        RAISERROR('Ya existe un médico con ese DNI.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Medico (
        EspecialidadCodigo, Dni, Nombre, Apellido,
        FechaIngreso, Domicilio, Localidad, ProvinciaCodigo,
        Telefono, Email, Guardia
    )
    VALUES (
        @EspecialidadCodigo, @Dni, @Nombre, @Apellido,
        @FechaIngreso, @Domicilio, @Localidad, @ProvinciaCodigo,
        @Telefono, @Email, @Guardia
    );

    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO
