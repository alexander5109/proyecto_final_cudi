CREATE TABLE [dbo].[Turno]
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [PacienteId] INT NOT NULL,
    MedicoId INT NOT NULL,
    Fecha DATE NOT NULL,
    Hora TIME NOT NULL,
    [DuracionMinutos] INT NOT NULL DEFAULT 40, 
    CONSTRAINT no_disponible_medico UNIQUE (MedicoId, Fecha, Hora),
    CONSTRAINT no_disponible_paciente UNIQUE ([PacienteId], Fecha, Hora),
    FOREIGN KEY ([PacienteId]) REFERENCES Paciente(Id),
    FOREIGN KEY (MedicoId) REFERENCES Medico(Id)
)
