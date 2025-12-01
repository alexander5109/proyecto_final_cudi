CREATE PROCEDURE [dbo].[sp_UpdatePaciente]
    @Id INT,
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

    UPDATE [dbo].[Paciente]
    SET 
        [Dni] = @Dni,
        [Nombre] = @Nombre,
        [Apellido] = @Apellido,
        [FechaIngreso] = @FechaIngreso,
        [Email] = @Email,
        [Telefono] = @Telefono,
        [FechaNacimiento] = @FechaNacimiento,
        [Domicilio] = @Domicilio,
        [Localidad] = @Localidad,
        [ProvinciaCodigo] = @ProvinciaCodigo
    WHERE [Id] = @Id;
END;
GO
