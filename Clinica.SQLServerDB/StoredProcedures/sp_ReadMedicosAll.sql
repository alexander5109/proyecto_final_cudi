CREATE PROCEDURE [dbo].[sp_ReadMedicosAll]
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
        m.ProvinciaCodigo,
        m.Telefono,
        m.EspecialidadCodigoInterno,
        m.Guardia,
        m.SueldoMinimoGarantizado,
        hm.Id AS HorarioId,
        hm.DiaSemana,
        hm.HoraDesde,
        hm.HoraHasta
    FROM Medico m
    LEFT JOIN HorarioMedico hm
        ON hm.MedicoId = m.Id
    ORDER BY hm.DiaSemana, hm.HoraDesde;
END;
GO
