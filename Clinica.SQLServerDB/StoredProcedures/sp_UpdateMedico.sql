CREATE PROCEDURE [dbo].[sp_UpdateMedico]
    @Id INT,
    @Name NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ProvinciaCodigo NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @EspecialidadCodigoInterno INT,
    @Telefono NVARCHAR(50),
    @Guardia BIT,
    @FechaIngreso DATE,
    @SueldoMinimoGarantizado DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Medico]
    SET 
        [Name] = @Name,
        [LastName] = @LastName,
        [Dni] = @Dni,
        [ProvinciaCodigo] = @ProvinciaCodigo,
        [Domicilio] = @Domicilio,
        [Localidad] = @Localidad,
        EspecialidadCodigoInterno = @EspecialidadCodigoInterno,
        [Telefono] = @Telefono,
        [Guardia] = @Guardia,
        [FechaIngreso] = @FechaIngreso,
        [SueldoMinimoGarantizado] = @SueldoMinimoGarantizado
    WHERE [Id] = @Id;
END;
GO
