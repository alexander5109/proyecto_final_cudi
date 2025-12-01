CREATE PROCEDURE [dbo].[sp_CreatePaciente]
    @Dni NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @FechaIngreso DATE,
    @Email NVARCHAR(200),
    @Telefono NVARCHAR(50),
    @FechaNacimiento DATE,
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @ProvinciaCodigo INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Paciente]
        ([Dni], [Nombre], [Apellido], [FechaIngreso], [Email],
         [Telefono], [FechaNacimiento], [Domicilio], [Localidad], [ProvinciaCodigo])
    VALUES
        (@Dni, @Nombre, @Apellido, @FechaIngreso, @Email,
         @Telefono, @FechaNacimiento, @Domicilio, @Localidad, @ProvinciaCodigo);

    SELECT SCOPE_IDENTITY() AS NuevoId;
END;
GO
