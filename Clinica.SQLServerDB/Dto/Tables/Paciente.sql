CREATE TABLE [dbo].[Paciente](
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Dni CHAR(8) NOT NULL UNIQUE, -- ej 20253532
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    FechaIngreso DATETIME2(0) NOT NULL, -- el datetime mas basico, no necesito microseconds
    Domicilio VARCHAR(50),
    Localidad VARCHAR(50),
    ProvinciaCodigo TINYINT,     -- provincias 1-24
    Telefono CHAR(10) NOT NULL,  -- 1138844770 sin espacios ni otra cosa
    Email VARCHAR(320),          -- mejor que NVARCHAR(50)
    FechaNacimiento DATE         -- mejor que datetime2(0)
);
