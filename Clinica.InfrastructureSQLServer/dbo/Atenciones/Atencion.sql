CREATE TABLE dbo.Atencion (
    Id INT NOT NULL PRIMARY KEY,         -- AtencionId
    TurnoId INT NOT NULL,               -- FK a Turno
    PacienteId INT NOT NULL,            -- FK a Paciente
    MedicoId INT NOT NULL,              -- FK a Medico
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),       -- Fecha de creación de la atención
    Observaciones NVARCHAR(MAX) NULL                 -- Comentarios/diagnóstico
);