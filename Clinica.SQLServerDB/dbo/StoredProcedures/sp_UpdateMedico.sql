CREATE PROCEDURE [dbo].[sp_UpdateMedico]
    @Id INT,
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

    UPDATE [dbo].[Medico]
    SET 
        [Name] = @Name,
        [LastName] = @LastName,
        [Dni] = @Dni,
        [Provincia] = @Provincia,
        [Domicilio] = @Domicilio,
        [Localidad] = @Localidad,
        [Especialidad] = @Especialidad,
        [Telefono] = @Telefono,
        [Guardia] = @Guardia,
        [FechaIngreso] = @FechaIngreso,
        [SueldoMinimoGarantizado] = @SueldoMinimoGarantizado
    WHERE [Id] = @Id;
END;
GO
