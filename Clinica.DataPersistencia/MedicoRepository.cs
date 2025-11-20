using Clinica.Dominio.Entidades;
using Clinica.Dominio.Comun;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;


public static class MedicoRepository {

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

		int newIdObj = int.Parse(comando.ExecuteScalar().ToString());
		return new Result<MedicoDto>.Ok(MedicoDto.FromDomain(instancia, newIdObj));
	}








	public static List<MedicoDto> GetAll(string connectionString) {
		var lista = new List<MedicoDto>();

		using SqlConnection conexion = new(connectionString);
		conexion.Open();

		using SqlCommand comando = new("sp_ReadMedicosAllWithHorarios", conexion);
		comando.CommandType = CommandType.StoredProcedure;

		using SqlDataReader reader = comando.ExecuteReader();

		while (reader.Read()) {
			lista.Add(MedicoDto.FromSQLReader(reader));
		}

		return lista;
	}








	public static bool Update(Medico2025WithId instanciaWithId, string connectionString) {
		using SqlConnection conexion = new(connectionString);
		conexion.Open();

		Medico2025 medico = instanciaWithId.Medico;
		MedicoId2025 medicoId = instanciaWithId.Id;

		// 1. Programar DataTable con los horarios del médico
		DataTable horariosTable = new();
		horariosTable.Columns.Add("CodigoInterno", typeof(int));
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
		comando.Parameters.AddWithValue("@CodigoInterno", medicoId);
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
		string query = "DELETE FROM Medico WHERE CodigoInterno = @CodigoInterno";
		using (SqlConnection connection = new(connectionString)) {
			connection.Open();
			using SqlCommand sqlComando = new(query, connection);
			sqlComando.Parameters.AddWithValue("@CodigoInterno", instanciaId);
			sqlComando.ExecuteNonQuery();
		}
		dictMedicos.Remove(instanciaId);
		return true;
	}
}
