CREATE PROCEDURE dbo.sp_InsertPaciente
    @Dni NCHAR(8),
    @Name NVARCHAR(50),
    @LastName NVARCHAR(50),
    @FechaIngreso DATETIME,
    @Domicilio NVARCHAR(50),
    @Localidad NVARCHAR(50),
    @Provincia NVARCHAR(50),
    @Telefono NVARCHAR(20),
    @Email NVARCHAR(50),
    @FechaNacimiento DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Paciente (Dni, Name, LastName, FechaIngreso, Domicilio, Localidad, Provincia, Telefono, Email, FechaNacimiento)
    VALUES (@Dni, @Name, @LastName, @FechaIngreso, @Domicilio, @Localidad, @Provincia, @Telefono, @Email, @FechaNacimiento);

    SELECT SCOPE_IDENTITY() AS PacienteId;
END
GO

