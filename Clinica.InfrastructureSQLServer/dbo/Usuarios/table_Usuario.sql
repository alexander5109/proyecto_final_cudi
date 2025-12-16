CREATE TABLE Usuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(20) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Apellido NVARCHAR(50) NOT NULL,
    Telefono CHAR(10) NOT NULL,
    Email VARCHAR(320) NOT NULL,
    EnumRole TINYINT NOT NULL CHECK (EnumRole BETWEEN 1 AND 10),
    MedicoRelacionadoId INT,
    CONSTRAINT FK_Usuario_Medico FOREIGN KEY (MedicoRelacionadoId) REFERENCES Medico(Id)
);
