/*
--------------------------------------------------------------------------------------
 Script posterior a la implementación (Post-Deployment)
--------------------------------------------------------------------------------------
 Este archivo se ejecuta luego de desplegar la base de datos.
 Usa sintaxis SQLCMD si querés incluir otros archivos:
 Ejemplo:  :r .\dbo\Tables\Paciente.sql
--------------------------------------------------------------------------------------
*/


----------------------------------------------------
-- Cargar datos de ejemplo en Medico
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Medico)
BEGIN
    SET IDENTITY_INSERT dbo.Medico ON;
    INSERT INTO dbo.Medico 
        (Id, Name, LastName, Provincia, Domicilio, Localidad, EspecialidadCodigoInterno, Telefono, Guardia, FechaIngreso, SueldoMinimoGarantizado, Dni)
    VALUES 
        (1, 'Dr. Ricardo', 'Arjona', 'Buenos Aires', 'Av. Siempre Viva 123', 'Capital Federal', 1, '1234567890', 1, '20220115', 285000.50, '12345678'),
        (2, 'Dr. Tocando', 'Shells', 'Córdoba', 'Calle Falsa 456', 'Villa Carlos Paz', 9, '2345678901', 0, '20210520', 392000.00, '87654321'),
        (3, 'Dr. Mario', 'Socolinsky', 'Mendoza', 'Ruta 40 Km 12', 'Godoy Cruz', 7, '3456789012', 1, '20200910', 378000.75, '11223344'),
        (4, 'Dra. Roxana', 'Toledo', 'Salta', 'Calle San Martin 100', 'Salta', 5, '4567890123', 0, '20230205', 399000.25, '55667788'),
        (5, 'Dra. Tete', 'Falopa', 'Santa Fe', 'Boulevard Galvez 2000', 'Rosario',  3, '5678901234', 1, '20191225', 486000.00, '99887766'),
        (6, 'Dra. Debora', 'Meltrozo', 'Buenos Aires', 'Ruta 40 Km 10', 'Uruguay', 2, '1238909252', 1, '20241005', 500000.00, '40350997'),
        (7, 'Dr. Miguel', 'DedoGordo', 'Buenos Aires', 'Ruta 40 Km 13', 'Italia', 4, '2348905216', 0, '20211003', 543555.00, '54355292'),
        (8, 'Dr. Felipe', 'Estomagón', 'Chaco', 'Av. Curva Peligrosa 78', 'Resistencia', 6, '6781234567', 1, '20230714', 655000.00, '65432198'),
        (9, 'Dr. Paco', 'Lespiedras', 'Misiones', 'Ruta de Tierra 99', 'Posadas', 8, '7892345678', 0, '20221101', 835000.75, '45678901'),
        (10, 'Dra. Clara', 'Mentoni', 'La Pampa', 'Calle Polvorienta 101', 'Santa Rosa', 10, '8903456789', 1, '20200420', 700500.25, '11225588');
    SET IDENTITY_INSERT dbo.Medico OFF;
END;
GO

----------------------------------------------------
-- Cargar datos de ejemplo en HorarioMedico
----------------------------------------------------


IF NOT EXISTS (SELECT 1 FROM dbo.HorarioMedico)
BEGIN
    INSERT INTO dbo.HorarioMedico (MedicoId, DiaSemana, HoraDesde, HoraHasta)
    VALUES
        (1, 3, '12:00', '17:00'),
        (1, 5, '12:00', '17:00'),
        (1, 2, '08:30', '12:00'),
        (1, 4, '08:30', '12:00'),
        (1, 6, '08:30', '12:00'),
        (2, 3, '10:00', '17:00'),
        (2, 5, '10:00', '17:00'),
        (3, 4, '11:00', '14:00'),
        (3, 2, '11:00', '14:00'),
        (5, 6, '09:00', '13:00'),
        (5, 6, '15:00', '17:00');
END;
GO

----------------------------------------------------
-- Cargar datos de ejemplo en Paciente
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Paciente)
BEGIN
    INSERT INTO dbo.Paciente 
        (Dni, Name, LastName, FechaIngreso, Domicilio, Localidad, ProvinciaCodigo, Telefono, Email, FechaNacimiento)
    VALUES 
        ('87654321', 'Ana', 'Gonzalez', '2023-04-15 09:30', 'Calle Flores 123', 1, 'Buenos Aires', '1234567890', 'ana.gonzalez@example.com', '1990-06-10'),
        ('12345678', 'Carlos', 'Pereira', '2022-11-30 14:45', 'Av. Libertad 456', 2, 'Santa Fe', '2345678901', 'carlos.pereira@example.com', '1985-02-18'),
        ('23456789', 'Maria', 'Lopez', '2024-01-05 08:00', 'San Martin 789',3, 'Córdoba', '3456789012', 'maria.lopez@example.com', '1992-09-25'),
        ('34567890', 'Juan', 'Martinez', '2021-08-20 10:15', 'Ruta 9 Km 15', 4, 'Mendoza', '4567890123', 'juan.martinez@example.com', '1978-12-05'),
        ('45678901', 'Sofia', 'Ramirez', '2023-03-10 12:30', 'Boulevard Galvez 200', 5, 'Salta', '5678901234', 'sofia.ramirez@example.com', '1988-04-22'),
        ('56789012', 'Lucia', 'Fernandez', '2023-05-18 16:30', 'Calle Belgrano 450', 6, 'Buenos Aires', '6789012345', 'lucia.fernandez@example.com', '1995-07-13');
END;
GO

----------------------------------------------------
-- Cargar datos de ejemplo en Turno
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Turno)
BEGIN
    INSERT INTO dbo.Turno (PacienteID, MedicoId, Fecha, Hora, DuracionMinutos)
    VALUES
        (1, 2, '2024-11-01', '09:00', 40),
        (1, 3, '2024-11-02', '09:30', 50),
        (2, 4, '2024-11-03', '10:00', 20),
        (3, 6, '2024-11-05', '11:00', 30),
        (5, 10, '2024-11-09', '13:00', 20);
END;
GO
