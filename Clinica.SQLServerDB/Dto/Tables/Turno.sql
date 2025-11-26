CREATE TABLE [dbo].[Turno](
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FechaDeCreacion DATETIME2(1) NOT NULL,
    PacienteId INT NOT NULL,
    MedicoId   INT NOT NULL,
    EspecialidadCodigo INT NOT NULL,
    FechaHoraAsignadaDesde DATETIME2(0) NOT NULL,
    FechaHoraAsignadaHasta DATETIME2(0) NOT NULL,
    OutcomeEstado TINYINT NOT NULL DEFAULT 1,     -- 1 = Programado (default)-- 2 = Cancelado-- 3 = Ausente-- 4 = Concretado-- 5 = Reprogramado
    OutcomeFecha DATETIME2(1) NULL,
    OutcomeComentario NVARCHAR(280) NULL,
    FOREIGN KEY (PacienteId) REFERENCES Paciente(Id),
    FOREIGN KEY (MedicoId) REFERENCES Medico(Id)
);
