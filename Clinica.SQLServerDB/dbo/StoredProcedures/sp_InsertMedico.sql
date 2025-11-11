CREATE PROCEDURE dbo.sp_InsertMedico
    @Name NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Dni NCHAR(8),
    @Provincia NVARCHAR(50),
    @Domicilio NVARCHAR(50),
    @Localidad NVARCHAR(50),
    @Especialidad NVARCHAR(50),
    @Telefono NVARCHAR(20),
    @Guardia BIT,
    @FechaIngreso DATETIME,
    @SueldoMinimoGarantizado FLOAT(53)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO dbo.Medico
        (
            Name, LastName, Dni, Provincia,
            Domicilio, Localidad, Especialidad,
            Telefono, Guardia, FechaIngreso, SueldoMinimoGarantizado
        )
        VALUES
        (
            @Name, @LastName, @Dni, @Provincia,
            @Domicilio, @Localidad, @Especialidad,
            @Telefono, @Guardia, @FechaIngreso, @SueldoMinimoGarantizado
        );

        -- devolver la nueva ID generada
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS MedicoId;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
