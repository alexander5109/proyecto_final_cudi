CREATE PROCEDURE [dbo].[sp_UpdatePaciente]
    @Id INT,
    @Dni NVARCHAR(20),
    @Name NVARCHAR(100),
    @LastName NVARCHAR(100),
    @FechaIngreso DATE,
    @Email NVARCHAR(200),
    @Telefono NVARCHAR(50),
    @FechaNacimiento DATE,
    @Domicilio NVARCHAR(200),
    @Localidad NVARCHAR(100),
    @Provincia NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Paciente]
    SET 
        [Dni] = @Dni,
        [Name] = @Name,
        [LastName] = @LastName,
        [FechaIngreso] = @FechaIngreso,
        [Email] = @Email,
        [Telefono] = @Telefono,
        [FechaNacimiento] = @FechaNacimiento,
        [Domicilio] = @Domicilio,
        [Localidad] = @Localidad,
        [Provincia] = @Provincia
    WHERE [Id] = @Id;
END;
GO
