CREATE TABLE dbo.Atencion (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    TurnoId INT NOT NULL,
    PacienteId INT NOT NULL,
    MedicoId INT NOT NULL,

    Fecha DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    Observaciones NVARCHAR(MAX) NULL,

    FOREIGN KEY (TurnoId) REFERENCES dbo.Turno (Id),
    FOREIGN KEY (PacienteId) REFERENCES dbo.Paciente (Id),
    FOREIGN KEY (MedicoId) REFERENCES dbo.Medico (Id)
);
