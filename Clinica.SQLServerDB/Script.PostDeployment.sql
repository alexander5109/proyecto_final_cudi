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
    INSERT INTO dbo.Medico 
        (Name, LastName, Provincia, Domicilio, Localidad, Especialidad, Telefono, Guardia, FechaIngreso, SueldoMinimoGarantizado, Dni)
    VALUES 
        ('Dr. Ricardo', 'Arjona', 'Buenos Aires', 'Av. Siempre Viva 123', 'Capital Federal', 'Cardiólogo', '123-456-7890', 1, '2022-01-15', 85000.50, '12345678'),
        ('Dr. Tocando', 'Shells', 'Córdoba', 'Calle Falsa 456', 'Villa Carlos Paz', 'Ginecólogo', '234-567-8901', 0, '2021-05-20', 92000.00, '87654321'),
        ('Dr. Mario', 'Socolinsky', 'Mendoza', 'Ruta 40 Km 12', 'Godoy Cruz', 'Pediatra', '345-678-9012', 1, '2020-09-10', 78000.75, '11223344'),
        ('Dra. Roxana', 'Toledo', 'Salta', 'Calle San Martin 100', 'Salta', 'Masajista', '456-789-0123', 0, '2023-02-05', 99000.25, '55667788'),
        ('Dra. Tete', 'Falopa', 'Santa Fe', 'Boulevard Galvez 2000', 'Rosario', 'Curadora', '567-890-1234', 1, '2019-12-25', 86000.00, '99887766'),
        ('Dra. Debora', 'Meltrozo', 'Buenos Aires', 'Ruta 40 Km 10', 'Uruguay', 'Traumatóloga', '123-890-9252', 1, '2024-10-05', 3564534.00, '40350997'),
        ('Dr. Miguel', 'DedoGordo', 'Buenos Aires', 'Ruta 40 Km 13', 'Italia', 'Proctólogo', '234-890-5216', 0, '2021-10-03', 543555.00, '54355292'),
        ('Dr. Felipe', 'Estomagón', 'Chaco', 'Av. Curva Peligrosa 78', 'Resistencia', 'Gastroenterólogo', '678-123-4567', 1, '2023-07-14', 75000.00, '65432198'),
        ('Dr. Paco', 'Lespiedras', 'Misiones', 'Ruta de Tierra 99', 'Posadas', 'Osteópata', '789-234-5678', 0, '2022-11-01', 83000.75, '45678901'),
        ('Dra. Clara', 'Mentoni', 'La Pampa', 'Calle Polvorienta 101', 'Santa Rosa', 'Cardióloga', '890-345-6789', 1, '2020-04-20', 70000.25, '11225588');
END;
GO

----------------------------------------------------
-- Cargar datos de ejemplo en Paciente
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Paciente)
BEGIN
    INSERT INTO dbo.Paciente 
        (Dni, Name, LastName, FechaIngreso, Domicilio, Localidad, Provincia, Telefono, Email, FechaNacimiento)
    VALUES 
        ('87654321', 'Ana', 'Gonzalez', '2023-04-15 09:30', 'Calle Flores 123', 'Buenos Aires', 'Buenos Aires', '123-456-7890', 'ana.gonzalez@example.com', '1990-06-10'),
        ('12345678', 'Carlos', 'Pereira', '2022-11-30 14:45', 'Av. Libertad 456', 'Rosario', 'Santa Fe', '234-567-8901', 'carlos.pereira@example.com', '1985-02-18'),
        ('23456789', 'Maria', 'Lopez', '2024-01-05 08:00', 'San Martin 789', 'Córdoba', 'Córdoba', '345-678-9012', 'maria.lopez@example.com', '1992-09-25'),
        ('34567890', 'Juan', 'Martinez', '2021-08-20 10:15', 'Ruta 9 Km 15', 'Mendoza', 'Mendoza', '456-789-0123', 'juan.martinez@example.com', '1978-12-05'),
        ('45678901', 'Sofia', 'Ramirez', '2023-03-10 12:30', 'Boulevard Galvez 200', 'Salta', 'Salta', '567-890-1234', 'sofia.ramirez@example.com', '1988-04-22'),
        ('56789012', 'Lucia', 'Fernandez', '2023-05-18 16:30', 'Calle Belgrano 450', 'La Plata', 'Buenos Aires', '678-901-2345', 'lucia.fernandez@example.com', '1995-07-13');
END;
GO

----------------------------------------------------
-- Cargar datos de ejemplo en Turno
----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Turno)
BEGIN
    INSERT INTO dbo.Turno (PacienteID, MedicoId, Fecha, Hora)
    VALUES
        (1, 2, '2024-11-01', '09:00'),
        (1, 3, '2024-11-02', '09:30'),
        (2, 4, '2024-11-03', '10:00'),
        (3, 6, '2024-11-05', '11:00'),
        (5, 10, '2024-11-09', '13:00');
END;
GO
