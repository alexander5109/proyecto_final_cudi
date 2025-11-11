CREATE PROCEDURE [dbo].[sp_CreateMedico]
    @Name NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Dni NVARCHAR(20),
    @Provincia NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @Especialidad NVARCHAR(100),
    @Telefono NVARCHAR(50),
    @Guardia BIT,
    @FechaIngreso DATE,
    @SueldoMinimoGarantizado DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Medico]
        ([Name], [LastName], [Dni], [Provincia], [Domicilio], [Localidad],
         [Especialidad], [Telefono], [Guardia], [FechaIngreso], [SueldoMinimoGarantizado])
    VALUES
        (@Name, @LastName, @Dni, @Provincia, @Domicilio, @Localidad,
         @Especialidad, @Telefono, @Guardia, @FechaIngreso, @SueldoMinimoGarantizado);

    SELECT SCOPE_IDENTITY() AS NuevoId;
END;
GO
