CREATE TABLE HorarioMedico (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MedicoId INT NOT NULL,
    DiaSemana INT NOT NULL, -- 1=Lunes, ..., 7=Domingo
    HoraDesde TIME NOT NULL,
    HoraHasta TIME NOT NULL,
    FOREIGN KEY (MedicoId) REFERENCES Medico(Id)
);