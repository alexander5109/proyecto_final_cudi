CREATE TABLE [dbo].[Medico](
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [EspecialidadCodigoInterno] TINYINT NOT NULL,
    Dni CHAR(8) NOT NULL UNIQUE,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    FechaIngreso DATETIME2(0) NOT NULL,
    Domicilio VARCHAR(50),
    Localidad VARCHAR(50),
    ProvinciaCodigo TINYINT,
    Telefono CHAR(10) NOT NULL,  -- 1138844770 sin espacios ni otra cosa
    Email VARCHAR(320),          -- mejor que NVARCHAR(50)
    Guardia BIT
)
