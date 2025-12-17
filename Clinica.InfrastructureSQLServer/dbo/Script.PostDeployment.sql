/*
--------------------------------------------------------------------------------------
 Script posterior a la implementación (Post-Deployment)
--------------------------------------------------------------------------------------
 Este archivo se ejecuta luego de desplegar la base de datos.
 Usa sintaxis SQLCMD si querés incluir otros archivos:
 Ejemplo:  :r .\dbo\Tables\Paciente.sql
--------------------------------------------------------------------------------------
*/


PRINT 'Ejecutando Post-Deployment...Haciendo unos inserts... incluyendo usuarios';



-- Índices recomendados
CREATE INDEX IX_Atencion_TurnoId ON Atencion(TurnoId);
CREATE INDEX IX_Atencion_PacienteId ON Atencion(PacienteId);
CREATE INDEX IX_Atencion_MedicoId ON Atencion(MedicoId);


----------------------------------------------------
-- Cargar datos de ejemplo en Medico
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Medico)
BEGIN
    SET IDENTITY_INSERT dbo.Medico ON;
    INSERT INTO dbo.Medico 
        (Id, Email, Nombre, Apellido, ProvinciaCodigo, Domicilio, Localidad, EspecialidadCodigo, Telefono, HaceGuardias, FechaIngreso, Dni)
    VALUES 
        (1, 'Ricardo@gmail.com', 'Dr. Ricardo', 'Arjona', 1, 'Av. Siempre Viva 123', 'Capital Federal', 1, '1234567890', 1, '20220115', '12345678'),
        (2, 'TocandoShells@gmail.com', 'Dr. Tocando', 'Shells', 2, 'Calle Falsa 456', 'Villa Carlos Paz', 9, '2345678901', 0, '20210520', '87654321'),
        (3, 'MarioPergo123@gmail.com', 'Dr. Mario', 'Socolinsky', 3, 'Ruta 40 Km 12', 'Godoy Cruz', 7, '3456789012', 1, '20200910', '11223344'),
        (4, 'Roxana123@gmail.com', 'Dra. Roxana', 'Toledo', 4, 'Calle San Martin 100', 'Salta', 5, '4567890123', 0, '20230205', '55667788'),
        (5, 'TeteFalopa@gmail.com', 'Dra. Tete', 'Falopa', 5, 'Boulevard Galvez 2000', 'Rosario',  3, '5678901234', 1, '20191225', '99887766'),
        (6, 'Debora123@gmail.com', 'Dra. Debora', 'Meltrozo', 6, 'Ruta 40 Km 10', 'Uruguay', 2, '1238909252', 1, '20241005', '40350997'),
        (7, 'Miguelito12@gmail.com', 'Dr. Miguel', 'DedoGordo', 7, 'Ruta 40 Km 13', 'Italia', 4, '2348905216', 0, '20211003', '54355292'),
        (8, 'Felipe123@gmail.com', 'Dr. Felipe', 'Estomagón', 8, 'Av. Curva Peligrosa 78', 'Resistencia', 6, '6781234567', 1, '20230714', '65432198'),
        (9, 'Paco123@gmail.com', 'Dr. Paco', 'Lespiedras', 9, 'Ruta de Tierra 99', 'Posadas', 8, '7892345678', 0, '20221101', '45678901'),
        (10, 'Clara123@gmail.com', 'Dra. Clara', 'Mentoni', 10, 'Calle Polvorienta 101', 'Santa Rosa', 10, '8903456789', 1, '20200420', '11225588');
    SET IDENTITY_INSERT dbo.Medico OFF;
END;
GO


----------------------------------------------------
-- Cargar datos de ejemplo en Usuario
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario)
BEGIN
	INSERT INTO dbo.Usuario 
		(UserName, PasswordHash, Nombre, Apellido, Telefono, Email, EnumRole, MedicoRelacionadoId)
	VALUES
		('super1', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 'Alexander', 'Seling', '1138830130', 'xanderseling@gmail.com', 1, NULL),
		('admin1', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 'Gerardo', 'Brokenhaüer', '1147835190', 'gerardobrokerhauer@gmail.com', 2, NULL),
		('secret1', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 'Roxana', 'Benitez', '1156830136', 'roxanabenitez@gmail.com', 3, NULL),
		('medico1', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 'Carlos', 'Merkier', '1164830132', 'carlosmerkier@gmail.com', 4, 1);
END;
GO

----------------------------------------------------


IF NOT EXISTS (SELECT 1 FROM dbo.Horario)
BEGIN
    INSERT INTO dbo.Horario 
        (MedicoId, DiaSemana, HoraDesde, HoraHasta, VigenteDesde, VigenteHasta)
    VALUES
        (1, 3, '12:00', '17:00', '2025-01-01', NULL),
        (1, 5, '12:00', '17:00', '2025-01-01', NULL),
        (1, 2, '08:30', '12:00', '2025-01-01', NULL),
        (1, 4, '08:30', '12:00', '2025-01-01', NULL),
        (1, 6, '08:30', '12:00', '2025-01-01', NULL),

        (2, 3, '10:00', '17:00', '2025-01-01', NULL),
        (2, 5, '10:00', '17:00', '2025-01-01', NULL),

        (3, 4, '11:00', '14:00', '2025-01-01', NULL),
        (3, 2, '11:00', '14:00', '2025-01-01', NULL),

        (5, 6, '09:00', '13:00', '2025-01-01', NULL),
        (5, 6, '15:00', '17:00', '2025-01-01', NULL);
END;
GO

----------------------------------------------------
-- Cargar datos de ejemplo en Paciente
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Paciente)
BEGIN
    INSERT INTO dbo.Paciente (Dni, Nombre, Apellido, FechaIngreso, Domicilio, ProvinciaCodigo, Localidad, Telefono, Email, FechaNacimiento)
    VALUES 
        ('87654321', 'Ana', 'Gonzalez', '2023-04-15 09:30', 'Calle Flores 123', 1, 'Gregorio de Laferrere', '1234567890', 'ana.gonzalez@example.com', '1990-06-10'),
        ('12345678', 'Carlos', 'Pereira', '2022-11-30 14:45', 'Av. Libertad 456', 2, 'Venado Tuerto', '2345678901', 'carlos.pereira@example.com', '1985-02-18'),
        ('23456789', 'Maria', 'Lopez', '2024-01-05 08:00', 'San Martin 789',3, 'El erizo', '3456789012', 'maria.lopez@example.com', '1992-09-25'),
        ('34567890', 'Juan', 'Martinez', '2021-08-20 10:15', 'Ruta 9 Km 15', 4, 'Las Flores', '4567890123', 'juan.martinez@example.com', '1978-12-05'),
        ('45678901', 'Sofia', 'Ramirez', '2023-03-10 12:30', 'Boulevard Galvez 200', 5, 'Gertrudis', '5678901234', 'sofia.ramirez@example.com', '1988-04-22'),
        ('56789012', 'Lucia', 'Fernandez', '2023-05-18 16:30', 'Calle Belgrano 450', 6, 'Uribelarrea', '6789012345', 'lucia.fernandez@example.com', '1995-07-13'),
        ('67890123', 'Federico', 'Alvarez', '2023-07-02 11:20', 'Calle Mitre 1200', 2, 'Villa Constitucion', '7890123456', 'federico.alvarez@example.com', '1983-03-15'),
        ('78901234', 'Valentina', 'Suarez', '2024-02-12 09:10', 'Av. Sarmiento 890', 3, 'La Carlota', '8901234567', 'valentina.suarez@example.com', '1998-11-03'),
        ('89012345', 'Bruno', 'Castillo', '2022-10-28 13:45', 'Calle Rivadavia 345', 1, 'Isidro Casanova', '9012345678', 'bruno.castillo@example.com', '1975-06-27'),
        ('90123456', 'Camila', 'Rojas', '2021-06-19 15:00', 'Av. Arenales 76', 4, 'San José del Rincon', '0123456789', 'camila.rojas@example.com', '1993-01-30'),
        ('01234567', 'Diego', 'Molina', '2023-12-05 17:10', 'Calle Alem 720', 6, 'Cañuelas', '1234509876', 'diego.molina@example.com', '1980-08-19'),
        ('13579246', 'Elena', 'Dominguez', '2024-03-22 08:50', 'Boulevard España 560', 5, 'Pueblo Esther', '2345610987', 'elena.dominguez@example.com', '1991-10-14');
END;
GO

----------------------------------------------------
-- Cargar datos de ejemplo en Turno
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Turno)
BEGIN
    INSERT INTO dbo.Turno 
        (FechaDeCreacion, PacienteId, MedicoId, EspecialidadCodigo, FechaHoraAsignadaDesde, FechaHoraAsignadaHasta, OutcomeEstado, OutcomeFecha, OutcomeComentario)
    VALUES
        ('2025-01-10 08:00', 1, 1, 1, '2025-01-15 09:00', '2025-01-15 09:30', 4, '2025-01-15 09:30', 'Ok'),
        ('2025-01-12 10:30', 2, 2, 9, '2025-01-18 11:00', '2025-01-18 11:30', 4, '2025-01-15 09:30', 'Ok'),

        ('2025-01-14 09:45', 3, 3, 7, '2025-01-20 08:30', '2025-01-20 09:00', 3, '2025-01-16 09:45', 'Cancelado por accidente'),
        ('2025-01-16 09:45', 3, 3, 7, '2025-01-24 08:30', '2025-01-24 09:00', 4, '2025-01-24 09:45', 'Ok'),

        ('2025-01-15 11:00', 4, 4, 5, '2025-01-22 10:00', '2025-01-22 10:30', 2, '2025-01-22 10:30', 'Ausente y no avisó'),
        ('2025-01-16 14:00', 5, 5, 3, '2025-01-25 14:30', '2025-01-25 15:00', 2, '2025-01-22 10:30', 'Ausente y no avisó'),
        ('2025-01-17 15:10', 6, 6, 2, '2025-01-26 16:00', '2025-01-26 16:30', 4, '2025-01-26 16:30', 'Ok'),
        ('2025-11-18 09:20', 7, 7, 4, '2026-01-28 09:00', '2026-01-28 09:30', 1, NULL, NULL),
        ('2025-11-18 11:40', 8, 8, 6, '2026-01-29 10:00', '2026-01-29 10:30', 1, NULL, NULL),
        ('2025-11-19 13:00', 9, 9, 8,'2026-01-30 11:00', '2026-01-30 11:30', 1, NULL, NULL),
        ('2025-11-21 15:30', 10, 10, 10, '2026-02-01 12:00', '2026-02-01 12:30', 1, NULL, NULL);
END;
GO
PRINT 'POST DEPLOY COMPLETO';