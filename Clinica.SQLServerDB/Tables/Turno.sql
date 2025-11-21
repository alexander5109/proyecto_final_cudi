CREATE TABLE [dbo].[Turno]
(
    -- Identificador del turno (slot reservado)
    Id INT IDENTITY(1,1) PRIMARY KEY,

    -- Relaciones con entidades del dominio
    FechaDeCreacion DATETIME NOT NULL,
    PacienteId INT NOT NULL,
    MedicoId   INT NOT NULL,
    EspecialidadCodigo INT NOT NULL,

    -- Datos de programación del turno
    FechaHoraAsignadaDesde DATETIME NOT NULL,
    FechaHoraAsignadaHasta DATETIME NOT NULL,

    -- ---------------------------
    -- Resultado final del turno
    -- ---------------------------
    -- 1 = Programado (default)
    -- 2 = Cancelado
    -- 3 = Ausente
    -- 4 = Concretado
    OutcomeEstado TINYINT NOT NULL DEFAULT 1,
    OutcomeFecha DATETIME NULL,
    OutcomeComentario NVARCHAR(280) NULL,

    -- ---------------------------
    -- Restricciones de unicidad
    -- ---------------------------
    -- Un médico no puede tener dos turnos que empiecen a la misma hora
    CONSTRAINT UQ_Turno_Medico UNIQUE (MedicoId, FechaHoraAsignadaDesde),

    -- Un paciente no puede tener dos turnos que empiecen a la misma hora
    CONSTRAINT UQ_Turno_Paciente UNIQUE (PacienteId, FechaHoraAsignadaHasta),

    -- ---------------------------
    -- Foreign Keys
    -- ---------------------------
    FOREIGN KEY (PacienteId) REFERENCES Paciente(Id),
    FOREIGN KEY (MedicoId) REFERENCES Medico(Id)
);
