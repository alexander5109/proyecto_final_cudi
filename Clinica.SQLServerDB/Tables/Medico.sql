CREATE TABLE [dbo].[Medico]
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [EspecialidadCodigoInterno] INT NOT NULL,
    Dni NCHAR(8) NOT NULL UNIQUE,
    Name NVARCHAR(50),
    LastName NVARCHAR(50),
    FechaIngreso DATETIME NOT NULL,
    Domicilio NVARCHAR(50),
    Localidad NVARCHAR(50),
    ProvinciaCodigo INT,
    Telefono NVARCHAR(20),
    Email NVARCHAR(50),
    Guardia BIT,
    SueldoMinimoGarantizado FLOAT(53)
)
