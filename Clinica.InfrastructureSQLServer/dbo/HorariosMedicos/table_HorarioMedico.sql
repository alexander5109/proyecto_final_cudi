CREATE TABLE dbo.Horario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MedicoId INT NOT NULL,
    DiaSemana TINYINT NOT NULL CHECK (DiaSemana BETWEEN 0 AND 6), -- DOMINGO 0, LUNES 1, SABADO 6
    HoraDesde TIME(0) NOT NULL,
    HoraHasta TIME(0) NOT NULL,
    VigenteDesde DATE NOT NULL,
    VigenteHasta DATE NULL,   -- NULL = vigente indefinidamente
    CONSTRAINT FK_Horario_Medico FOREIGN KEY (MedicoId) REFERENCES Medico(Id),
    CONSTRAINT CK_Horario_FrancaValida CHECK (HoraDesde < HoraHasta),
    CONSTRAINT CK_Horario_VigenciaValida CHECK (
        VigenteHasta IS NULL OR VigenteDesde <= VigenteHasta
    )
);
