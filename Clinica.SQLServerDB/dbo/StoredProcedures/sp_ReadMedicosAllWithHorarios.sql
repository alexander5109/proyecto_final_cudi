CREATE PROCEDURE [dbo].sp_ReadMedicosAllWithHorarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        m.Id,
        m.Dni,
        m.Name,
        m.LastName,
        m.FechaIngreso,
        m.Domicilio,
        m.Localidad,
        m.Provincia,
        m.Telefono,
        m.Especialidad,
        m.Guardia,
        m.SueldoMinimoGarantizado,
        (
            SELECT 
                hm.DiaSemana,
                (
                    SELECT hm2.HoraDesde, hm2.HoraHasta
                    FROM HorarioMedico hm2
                    WHERE hm2.MedicoId = m.Id AND hm2.DiaSemana = hm.DiaSemana
                    FOR JSON PATH
                ) AS HorariosDia
            FROM HorarioMedico hm
            WHERE hm.MedicoId = m.Id
            GROUP BY hm.DiaSemana
            FOR JSON PATH
        ) AS HorariosPorDia
    FROM Medico m
    ORDER BY m.Id;
END;
GO
