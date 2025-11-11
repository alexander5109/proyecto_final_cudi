CREATE TABLE [dbo].[Paciente]
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Dni NCHAR(8) NOT NULL UNIQUE,
    Name NVARCHAR(50),
    LastName NVARCHAR(50),
    FechaIngreso DATETIME NOT NULL,
    Domicilio NVARCHAR(50),
    Localidad NVARCHAR(50),
    Provincia NVARCHAR(50),
    Telefono NVARCHAR(20),
    Email NVARCHAR(50),
    FechaNacimiento DATETIME
)
