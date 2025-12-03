CREATE PROCEDURE dbo.[sp_UpdateMedico]
    @Id INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni NVARCHAR(20),
    @ProvinciaCodigo NVARCHAR(100),
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @EspecialidadCodigoInterno INT,
    @Telefono NVARCHAR(50),
    @Email NVARCHAR(150),
    @Guardia BIT,
    @FechaIngreso DATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.[Medico]
    SET 
        [Nombre] = @Nombre,
        [Apellido] = @Apellido,
        [Dni] = @Dni,
        [ProvinciaCodigo] = @ProvinciaCodigo,
        [Domicilio] = @Domicilio,
        [Localidad] = @Localidad,
        EspecialidadCodigoInterno = @EspecialidadCodigoInterno,
        [Telefono] = @Telefono,
        [Email] = @Email,
        [Guardia] = @Guardia,
        [FechaIngreso] = @FechaIngreso
    WHERE [Id] = @Id;
END;
GO
