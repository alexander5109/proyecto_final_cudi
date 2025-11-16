using Clinica.DataPersistencia.ModelDtos;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Clinica.DataPersistencia.Repositorios;

public class MedicosRepositoriosOld {

	public static Result<MedicoDto> Create(Medico2025 instancia, string connectionString) {
		using SqlConnection connection = new(connectionString);
		connection.Open();

		using SqlCommand comando = new("sp_CreateMedicoWithHorarios", connection);
		comando.CommandType = CommandType.StoredProcedure;

		// -----------------------------
		// 1. Parámetros escalares
		// -----------------------------
		comando.Parameters.AddWithValue("@Name", instancia.NombreCompleto.Nombre);
		comando.Parameters.AddWithValue("@LastName", instancia.NombreCompleto.Apellido);
		comando.Parameters.AddWithValue("@Dni", instancia.Dni.Valor);
		comando.Parameters.AddWithValue("@Provincia", instancia.Domicilio.Localidad.Provincia.Nombre);
		comando.Parameters.AddWithValue("@Domicilio", instancia.Domicilio.Direccion);
		comando.Parameters.AddWithValue("@Localidad", instancia.Domicilio.Localidad.Nombre);
		comando.Parameters.AddWithValue("@Especialidad", instancia.Especialidad.Titulo);
		comando.Parameters.AddWithValue("@Telefono", instancia.Telefono.Valor);
		comando.Parameters.AddWithValue("@Guardia", instancia.HaceGuardias);
		comando.Parameters.AddWithValue("@FechaIngreso", instancia.FechaIngreso.Valor);
		comando.Parameters.AddWithValue("@SueldoMinimoGarantizado", instancia.SueldoMinimoGarantizado.Valor);

		// -----------------------------
		// 2. UDTT: Tabla de Horarios
		// -----------------------------
		DataTable horariosTable = new();
		horariosTable.Columns.Add("DiaSemana", typeof(int));
		horariosTable.Columns.Add("HoraDesde", typeof(TimeSpan));
		horariosTable.Columns.Add("HoraHasta", typeof(TimeSpan));

		foreach (HorarioMedico2025 horario in instancia.ListaHorarios.Valores) {
			horariosTable.Rows.Add(
				(int)horario.DiaSemana.Valor,
				horario.Desde.Valor,
				horario.Hasta.Valor
			);
		}

		SqlParameter paramHorarios = comando.Parameters.AddWithValue("@Horarios", horariosTable);
		paramHorarios.SqlDbType = SqlDbType.Structured;
		paramHorarios.TypeName = "dbo.HorarioMedicoUDTT";

		string newIdObj = comando.ExecuteScalar()?.ToString() ?? throw new Exception("El stored procedure no devolvió un Id.");
		return new Result<MedicoDto>.Ok(MedicoDto.FromDomain(instancia, newIdObj));
	}

	public static Dictionary<string, MedicoDto> ReadTodos(string connectionString) {
		using SqlConnection conexion = new(connectionString);
		conexion.Open();

		using SqlCommand comando = new("sp_ReadMedicosAllWithHorarios", conexion);
		comando.CommandType = CommandType.StoredProcedure;

		using SqlDataReader reader = comando.ExecuteReader();

		Dictionary<string, MedicoDto> dictMedicos = [];

		while (reader.Read()) {
			MedicoDto medicoDto = MedicoDto.FromSQLReader(reader);
			dictMedicos[medicoDto.Id] = medicoDto;
		}
		return dictMedicos;
	}
	public static bool Update(Medico2025WithId instanciaWithId, string connectionString) {
		using SqlConnection conexion = new(connectionString);
		conexion.Open();

		Medico2025 medico = instanciaWithId.Medico;
		MedicoId2025 medicoId = instanciaWithId.Id;

		// 1. Crear DataTable con los horarios del médico
		DataTable horariosTable = new();
		horariosTable.Columns.Add("Id", typeof(int));
		horariosTable.Columns.Add("DiaSemana", typeof(int));
		horariosTable.Columns.Add("HoraDesde", typeof(TimeSpan));
		horariosTable.Columns.Add("HoraHasta", typeof(TimeSpan));

		foreach (HorarioMedico2025 h in medico.ListaHorarios.Valores) {
			horariosTable.Rows.Add(
				(int)h.DiaSemana.Valor,
				h.Desde.Valor,
				h.Hasta.Valor
			);
		}

		using SqlCommand comando = new("sp_UpdateMedicoWithHorarios", conexion);
		comando.CommandType = CommandType.StoredProcedure;

		// 2. Agregar parámetros del médico
		comando.Parameters.AddWithValue("@Id", medicoId);
		comando.Parameters.AddWithValue("@Name", medico.NombreCompleto.Nombre);
		comando.Parameters.AddWithValue("@LastName", medico.NombreCompleto.Apellido);
		comando.Parameters.AddWithValue("@Dni", medico.Dni.Valor);
		comando.Parameters.AddWithValue("@Provincia", medico.Domicilio.Localidad.Provincia.Nombre);
		comando.Parameters.AddWithValue("@Domicilio", medico.Domicilio.Direccion);
		comando.Parameters.AddWithValue("@Localidad", medico.Domicilio.Localidad.Nombre);
		comando.Parameters.AddWithValue("@Especialidad", medico.Especialidad.Titulo);
		comando.Parameters.AddWithValue("@Telefono", medico.Telefono.Valor);
		comando.Parameters.AddWithValue("@Guardia", medico.HaceGuardias);
		comando.Parameters.AddWithValue("@FechaIngreso", medico.FechaIngreso.Valor);
		comando.Parameters.AddWithValue("@SueldoMinimoGarantizado", medico.SueldoMinimoGarantizado.Valor);

		// 3. Agregar la tabla de horarios (UDTT)
		SqlParameter horariosParam = comando.Parameters.AddWithValue("@Horarios", horariosTable);
		horariosParam.SqlDbType = SqlDbType.Structured;
		horariosParam.TypeName = "HorarioMedicoType"; // nombre exacto del UDTT en SQL Server

		// 4. Ejecutar
		comando.ExecuteNonQuery();

		return true;
	}
	public static bool Delete(Dictionary<string, MedicoDto> dictMedicos, string instanciaId, string connectionString) {
		string query = "DELETE FROM Medico WHERE Id = @Id";
		using (SqlConnection connection = new(connectionString)) {
			connection.Open();
			using SqlCommand sqlComando = new(query, connection);
			sqlComando.Parameters.AddWithValue("@Id", instanciaId);
			sqlComando.ExecuteNonQuery();
		}
		dictMedicos.Remove(instanciaId);
		return true;
	}
}
