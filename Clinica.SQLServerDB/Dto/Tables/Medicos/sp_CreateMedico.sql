CREATE PROCEDURE [dbo].[sp_CreateMedico]
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ProvinciaCodigo NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @EspecialidadCodigoInterno INT,
    @Telefono NVARCHAR(50),
    @Guardia BIT,
    @FechaIngreso DATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Medico]
        ([Nombre], [Apellido], [Dni], [ProvinciaCodigo], [Domicilio], [Localidad],
         EspecialidadCodigoInterno, [Telefono], [Guardia], [FechaIngreso])
    VALUES
        (@Nombre, @Apellido, @Dni, @ProvinciaCodigo, @Domicilio, @Localidad,
         @EspecialidadCodigoInterno, @Telefono, @Guardia, @FechaIngreso);

    SELECT SCOPE_IDENTITY() AS NuevoId;
END;
GO
