/*
 Plantilla de script anterior a la implementación							
--------------------------------------------------------------------------------------
 Este archivo contiene instrucciones de SQL que se ejecutarán antes del script de compilación	
 Use la sintaxis de SQLCMD para incluir un archivo en el script anterior a la implementación			
 Ejemplo:      :r .\miArchivo.sql								
 Use la sintaxis de SQLCMD para hacer referencia a una variable en el script anterior a la implementación		
 Ejemplo:      :setvar TableName miTabla							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/*
    PRINT 'Ejecutando Pre-Deployment... Limpiando tablas existentes';
    IF OBJECT_ID('Turno', 'U') IS NOT NULL
        DROP TABLE Turno;
    IF OBJECT_ID('Paciente', 'U') IS NOT NULL
        DROP TABLE Paciente;
    IF OBJECT_ID('Medico', 'U') IS NOT NULL
        DROP TABLE Medico;
*/