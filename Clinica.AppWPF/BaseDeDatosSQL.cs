using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;


namespace Clinica.AppWPF;
public class BaseDeDatosSQL : BaseDeDatosAbstracta {
	private string connectionString;
	static readonly string _scriptClinicaMedicaCreaTablasConInserts = "databases/_scriptClinicaMedicaCreaTablasConInserts.sql";

	public BaseDeDatosSQL(string? customConnectionString = null) {
		ConectadaExitosamente =
			AsegurarArchivoAppConfig(customConnectionString)
			&& CadenaDeConexionEsValida()
			&& SQLCargarMedicosExitosamente()
			&& SQLCargarPacientesExitosamente()
			&& SQLCargarTurnosExitosamente()
		;
	}

	//------------------------public.CREATE.ModelViewMedico----------------------//
	public override bool CreateMedico(Medico2025 instancia, ModelViewMedico instanceDto) {
		string insertQuery = @"
				INSERT INTO Medico (Name, LastName, Dni, Provincia, Domicilio, Localidad, Especialidad, Telefono, Guardia, FechaIngreso, SueldoMinimoGarantizado) 
				VALUES (@Name, @LastName, @Dni, @Provincia, @Domicilio, @Localidad, @Especialidad, @Telefono, @Guardia, @FechaIngreso, @SueldoMinimoGarantizado)
				SELECT SCOPE_IDENTITY();"; // DEVOLEME RAPIDAMENTE LA ID QUE ACABAS DE GENERAR

		try {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(insertQuery, connection)) {
					sqlComando.Parameters.AddWithValue("@Name", instancia.NombreCompleto.Nombre);
					sqlComando.Parameters.AddWithValue("@LastName", instancia.NombreCompleto.Apellido);
					sqlComando.Parameters.AddWithValue("@Dni", instancia.Dni.Valor);
					sqlComando.Parameters.AddWithValue("@Provincia", instancia.Domicilio.Localidad.Provincia.Nombre);
					sqlComando.Parameters.AddWithValue("@Domicilio", instancia.Domicilio.Direccion);
					sqlComando.Parameters.AddWithValue("@Localidad", instancia.Domicilio.Localidad.Nombre);
					sqlComando.Parameters.AddWithValue("@Especialidad", instancia.Especialidad.Titulo);
					sqlComando.Parameters.AddWithValue("@Telefono", instancia.Telefono.Valor);
					sqlComando.Parameters.AddWithValue("@Guardia", instancia.HaceGuardias);
					sqlComando.Parameters.AddWithValue("@FechaIngreso", instancia.FechaIngreso.Valor);
					sqlComando.Parameters.AddWithValue("@SueldoMinimoGarantizado", instancia.SueldoMinimoGarantizado.Valor);
					instanceDto.Id = sqlComando.ExecuteScalar().ToString(); //ahora la instancia creada desde la ventana tiene su propia Id
				}
			}
			DictMedicos[instanceDto.Id] = instanceDto;
			// MessageBox.Show($"Exito: Se ha creado la instancia de ModelViewMedico: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation error code
		  {
			MessageBox.Show("Error de constraints. Ya existe un médico con ese dni.", "Violación de Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error code
		  {
			MessageBox.Show("No se puede crear el médico debido a una violación de clave foránea.", "Violación de Clave Foránea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}


	//------------------------public.CREATE.ModelViewPaciente----------------------//
	public override bool CreatePaciente(Paciente2025 instancia, ModelViewPaciente instanceDto) {
		string insertQuery = @"
				INSERT INTO Paciente (Dni, Name, LastName, FechaIngreso, Email, Telefono, FechaNacimiento, Domicilio, Localidad, Provincia) 
				VALUES (@Dni, @Name, @LastName, @FechaIngreso, @Email, @Telefono, @FechaNacimiento, @Domicilio, @Localidad, @Provincia)
				SELECT SCOPE_IDENTITY();"; // DEVOLEME RAPIDAMENTE LA ID QUE ACABAS DE GENERAR

		try {

			using (SqlConnection connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(insertQuery, connection)) {
					sqlComando.Parameters.AddWithValue("@Dni", instancia.Dni.Valor);
					sqlComando.Parameters.AddWithValue("@Name", instancia.NombreCompleto.Nombre);
					sqlComando.Parameters.AddWithValue("@LastName", instancia.NombreCompleto.Apellido);
					sqlComando.Parameters.AddWithValue("@FechaIngreso", DateTime.Today.ToString());
					sqlComando.Parameters.AddWithValue("@Email", instancia.Contacto.Email.Valor);
					sqlComando.Parameters.AddWithValue("@Telefono", instancia.Contacto.Telefono.Valor);
					sqlComando.Parameters.AddWithValue("@FechaNacimiento", instancia.FechaNacimiento.Valor);
					sqlComando.Parameters.AddWithValue("@Domicilio", instancia.Domicilio.Direccion);
					sqlComando.Parameters.AddWithValue("@Localidad", instancia.Domicilio.Localidad.Nombre);
					sqlComando.Parameters.AddWithValue("@Provincia", instancia.Domicilio.Localidad.Provincia.Nombre);
					instanceDto.Id = sqlComando.ExecuteScalar()?.ToString();    //ahora la instancia creada desde la ventana tiene su propia Id
				}
			}

			DictPacientes[instanceDto.Id] = instanceDto;
			// MessageBox.Show($"Exito: Se ha creado la instancia de ModelViewPaciente: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation error code
		  {
			MessageBox.Show("Error de constraints. Ya existe un paciente con ese dni.", "Violación de Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error code
		  {
			MessageBox.Show("No se puede crear el paciente debido a una violación de clave foránea.", "Violación de Clave Foránea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}


	//------------------------public.CREATE.ModelViewTurno----------------------//
	public override bool CreateTurno(Turno2025 instanciaValidada, ModelViewTurno instancia) {
		string insertQuery = @"
				INSERT INTO Turno (PacienteId, MedicoId, Fecha, Hora) 
				VALUES (@PacienteId, @MedicoId, @Fecha, @Hora);
				SELECT SCOPE_IDENTITY();"; // DEVOLEME RAPIDAMENTE LA ID QUE ACABAS DE GENERAR
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (var sqlComando = new SqlCommand(insertQuery, connection)) {
					sqlComando.Parameters.AddWithValue("@PacienteId", instancia.PacienteId);
					sqlComando.Parameters.AddWithValue("@MedicoId", instancia.MedicoId);
					sqlComando.Parameters.AddWithValue("@Fecha", instancia.Fecha);
					sqlComando.Parameters.AddWithValue("@Hora", instancia.Hora);
					instancia.Id = sqlComando.ExecuteScalar().ToString();   //ahora la instancia creada desde la ventana tiene su propia Id
				}
			}
			DictTurnos[instancia.Id] = instancia;
			//MessageBox.Show($"Exito: Se ha creado la instancia de ModelViewTurno con id: {instancia.Id} entre {instancia.PacienteId} {instancia.MedicoId} el dia {instancia.Fecha} a las {instancia.Hora}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation error code
		  {
			MessageBox.Show("Error de constraints. Ya existe un turno entre este paciente y medicoView en esa fecha. O el medicoView ya tiene un turno en esa fecha con otro paciente.", "Violación de Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error code
		  {
			MessageBox.Show("No se puede crear el turno debido a una violación de clave foránea.", "Violación de Clave Foránea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}


	//------------------------public.READ----------------------//
	public override List<ModelViewMedico> ReadMedicos() {
		return DictMedicos.Values.ToList();
	}

	public override List<ModelViewPaciente> ReadPacientes() {
		return DictPacientes.Values.ToList();
	}

	public override List<ModelViewTurno> ReadTurnos() {
		return DictTurnos.Values.ToList();
	}











	//------------------------public.UPDATE.ModelViewMedico----------------------//
	public override bool UpdateMedico(Medico2025 instancia, string instanceId) {
		string query = "UPDATE Medico SET Name = @Name, LastName = @LastName, Dni = @Dni, Provincia = @Provincia, Domicilio = @Domicilio, Localidad = @Localidad, Especialidad = @Especialidad, Telefono = @Telefono, Guardia = @Guardia, FechaIngreso = @FechaIngreso, SueldoMinimoGarantizado = @SueldoMinimoGarantizado WHERE Id = @Id";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (var sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@Name", instancia.NombreCompleto.Nombre);
					sqlComando.Parameters.AddWithValue("@LastName", instancia.NombreCompleto.Apellido);
					sqlComando.Parameters.AddWithValue("@Dni", instancia.Dni.Valor);
					sqlComando.Parameters.AddWithValue("@Provincia", instancia.Domicilio.Localidad.Provincia.Nombre);
					sqlComando.Parameters.AddWithValue("@Domicilio", instancia.Domicilio.Direccion);
					sqlComando.Parameters.AddWithValue("@Localidad", instancia.Domicilio.Localidad.Nombre);
					sqlComando.Parameters.AddWithValue("@Especialidad", instancia.Especialidad.Titulo);
					sqlComando.Parameters.AddWithValue("@Telefono", instancia.Telefono.Valor);
					sqlComando.Parameters.AddWithValue("@Guardia", instancia.HaceGuardias);
					sqlComando.Parameters.AddWithValue("@FechaIngreso", instancia.FechaIngreso.Valor);
					sqlComando.Parameters.AddWithValue("@SueldoMinimoGarantizado", instancia.SueldoMinimoGarantizado.Valor);
					sqlComando.Parameters.AddWithValue("@Id", instanceId);
					sqlComando.ExecuteNonQuery();
				}
			}
			// MessageBox.Show($"Exito: Se han actualizado los datos de: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation error code
		  {
			MessageBox.Show("Error de constraints. Ya existe un médico con ese dni.", "Violación de Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error code
		  {
			MessageBox.Show("No se puede crear el médico debido a una violación de clave foránea.", "Violación de Clave Foránea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}
	//------------------------public.UPDATE.ModelViewPaciente----------------------//
	public override bool UpdatePaciente(Paciente2025 instancia, string instanceId) {
		string query = "UPDATE Paciente SET Dni = @Dni, Name = @Name, LastName = @LastName, FechaIngreso = @FechaIngreso, Email = @Email, Telefono = @Telefono, FechaNacimiento = @FechaNacimiento, Domicilio = @Domicilio, Localidad = @Localidad, Provincia = @Provincia WHERE Id = @Id";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@Dni", instancia.Dni.Valor);
					sqlComando.Parameters.AddWithValue("@Name", instancia.NombreCompleto.Nombre);
					sqlComando.Parameters.AddWithValue("@LastName", instancia.NombreCompleto.Apellido);
					sqlComando.Parameters.AddWithValue("@FechaIngreso", instancia.FechaIngreso.Valor);
					sqlComando.Parameters.AddWithValue("@Email", instancia.Contacto.Email.Valor);
					sqlComando.Parameters.AddWithValue("@Telefono", instancia.Contacto.Telefono.Valor);
					sqlComando.Parameters.AddWithValue("@FechaNacimiento", instancia.FechaNacimiento.Valor);
					sqlComando.Parameters.AddWithValue("@Domicilio", instancia.Domicilio.Direccion);
					sqlComando.Parameters.AddWithValue("@Localidad", instancia.Domicilio.Localidad.Nombre);
					sqlComando.Parameters.AddWithValue("@Provincia", instancia.Domicilio.Localidad.Provincia.Nombre);
					sqlComando.Parameters.AddWithValue("@Id", instanceId);
					sqlComando.ExecuteNonQuery();
				}
			}
			// MessageBox.Show($"Exito: Se han actualizado los datos de: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation error code
		  {
			MessageBox.Show("Error de constraints. Ya existe un paciente con ese dni.", "Violación de Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error code
		  {
			MessageBox.Show("No se puede crear el paciente debido a una violación de clave foránea.", "Violación de Clave Foránea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}
	//------------------------public.UPDATE.ModelViewTurno----------------------//
	public override bool UpdateTurno(Turno2025 instanciaValidada, ModelViewTurno instancia) {
		string query = "UPDATE Turno SET PacienteId = @PacienteId, MedicoId = @MedicoId, Fecha = @Fecha, Hora = @Hora WHERE Id = @Id";
		// string query = "UPDATE ModelViewTurno SET PacienteId = @PacienteId, MedicoId = @MedicoId WHERE Id = @Id";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@PacienteId", instancia.PacienteId);
					sqlComando.Parameters.AddWithValue("@MedicoId", instancia.MedicoId);
					sqlComando.Parameters.AddWithValue("@Fecha", instancia.Fecha);
					sqlComando.Parameters.AddWithValue("@Hora", instancia.Hora);
					sqlComando.Parameters.AddWithValue("@Id", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			//MessageBox.Show($"Exito: Se han actualizado los datos del turno con id: {instancia.Id}. Ahora entre {instancia.PacienteId} {instancia.MedicoId} el dia {instancia.Fecha} a las {instancia.Hora}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation error code
		  {
			MessageBox.Show("Error de constraints. Ya existe un turno entre este paciente y medicoView en esa fecha. O el medicoView ya tiene un turno en esa fecha con otro paciente.", "Violación de Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error code
		  {
			MessageBox.Show("No se puede crear el turno debido a una violación de clave foránea.", "Violación de Clave Foránea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}








	//------------------------public.DELETE.ModelViewMedico----------------------//
	public override bool DeleteMedico(ModelViewMedico instancia) {
		string query = "DELETE FROM Medico WHERE Id = @Id";

		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@Id", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			DictMedicos.Remove(instancia.Id);
			// MessageBox.Show($"Exito: Se ha eliminado el medicoView con id: {instancia.Id} de la Base de Datos SQL", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 547) // SQL Server foreign key violation error code
		  {
			MessageBox.Show("No se puede eliminar este medicoView porque tiene turnos asignados.", "Violacion de clave foranea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Data Base", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}
	//------------------------public.DELETE.ModelViewPaciente----------------------//
	public override bool DeletePaciente(ModelViewPaciente instancia) {
		string query = "DELETE FROM Paciente WHERE Id = @Id";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@Id", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			DictPacientes.Remove(instancia.Id);
			// MessageBox.Show($"Exito: Se ha eliminado el paciente con id: {instancia.Id} de la Base de Datos SQL", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 547) // SQL Server foreign key violation error code
		  {
			MessageBox.Show("No se puede eliminar este paciente porque tiene turnos asignados.", "Violacion de clave foranea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Data Base", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}
	//------------------------public.DELETE.ModelViewTurno----------------------//
	public override bool DeleteTurno(ModelViewTurno instancia) {
		string query = "DELETE FROM Turno WHERE Id = @Id";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@Id", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			DictTurnos.Remove(instancia.Id);
			// MessageBox.Show($"Exito: Se ha eliminado el turno con id: {instancia.Id} de la Base de Datos SQL", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
			return true;
		} catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation error code
		  {
			MessageBox.Show("Error de constraints. Ya existe un turno entre este paciente y medicoView en esa fecha. O el medicoView ya tiene un turno en esa fecha con otro paciente.", "Violación de Constraint", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error code
		  {
			MessageBox.Show("No se puede modificar el turno debido a una violación de clave foránea.", "Violación de Clave Foránea", MessageBoxButton.OK, MessageBoxImage.Warning);
		} catch (SqlException ex) {
			MessageBox.Show($"SQL error: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		return false;
	}









	//------------------------private.LOAD.WindowListarMedicos----------------------//
	private bool SQLCargarMedicosExitosamente() {
		try {
			using var conexion = new SqlConnection(connectionString);
			conexion.Open();

			using var comando = new SqlCommand("sp_ReadMedicosAllWithHorarios", conexion);
			comando.CommandType = CommandType.StoredProcedure;

			using var reader = comando.ExecuteReader();

			var dictMedicos = new Dictionary<string, ModelViewMedico>();

			while (reader.Read()) {

				string medicoId = reader["Id"].ToString()!;

				// -----------------------------------------
				// 1) Crear médico si no existe aún
				// -----------------------------------------
				if (!dictMedicos.TryGetValue(medicoId, out var medicoView)) {
					medicoView = new ModelViewMedico(
						[],                              // Horarios vacío
						medicoId,
						reader["Name"] as string,
						reader["LastName"] as string,
						reader["Dni"] as string,
						reader["Provincia"] as string,
						reader["Domicilio"] as string,
						reader["Localidad"] as string,
						reader["Especialidad"] as string,
						reader["Telefono"] as string,
						reader["Guardia"] != DBNull.Value ? Convert.ToBoolean(reader["Guardia"]) : null,
						reader["FechaIngreso"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIngreso"]) : null,
						reader["SueldoMinimoGarantizado"] != DBNull.Value ? Convert.ToDecimal(reader["SueldoMinimoGarantizado"]) : null
					);

					dictMedicos[medicoId] = medicoView;
				}

				// -----------------------------------------
				// 2) Procesar horarios (si existen)
				// -----------------------------------------
				if (reader["HorarioId"] != DBNull.Value) {

					// DiaSemana es INT 0..6 → convertir a DayOfWeek
					int diaInt = Convert.ToInt32(reader["DiaSemana"]);
					DayOfWeek diaEnum = (DayOfWeek)diaInt;

					// Construir tu tipo fuerte DiaSemana2025
					var diaSemana = new DiaSemana2025(diaEnum);

					// Leer horas
					var desde = TimeOnly.Parse(reader["HoraDesde"].ToString()!);
					var hasta = TimeOnly.Parse(reader["HoraHasta"].ToString()!);

					// Agregar horario directamente
					medicoView.Horarios.Add(new ModelViewHorario {
						DiaSemana = diaSemana.Valor,
						Desde = desde,
						Hasta = hasta
					});
				}
			}

			DictMedicos = dictMedicos;
			return true;
		} catch (Exception ex) {
			MessageBox.Show(
				$"Ocurrió un error al leer los médicos desde SQL: {ex.Message}",
				"Error de Database",
				MessageBoxButton.OK,
				MessageBoxImage.Error
			);

			return CrearLasTablasExitosamente();
		}
	}




	//------------------------private.LOAD.WindowListarPacientes----------------------//
	private bool SQLCargarPacientesExitosamente() {
		try {
			using (var conexion = new SqlConnection(connectionString)) {
				conexion.Open();
				string consulta = "SELECT * FROM Paciente";
				using (var sqlComando = new SqlCommand(consulta, conexion))
				using (var reader = sqlComando.ExecuteReader()) {
					while (reader.Read()) {
						var paciente = new ModelViewPaciente(
							Convert.ToString(reader["Id"]) ?? string.Empty,
							Convert.ToString(reader["Dni"]),
							Convert.ToString(reader["Name"]),
							Convert.ToString(reader["LastName"]),
							reader["FechaIngreso"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIngreso"]) : null,
							Convert.ToString(reader["Email"]),
							Convert.ToString(reader["Telefono"]),
							reader["FechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(reader["FechaNacimiento"]) : null,
							Convert.ToString(reader["Domicilio"]),
							Convert.ToString(reader["Localidad"]),
							Convert.ToString(reader["Provincia"])
						);
						//MessageBox.Show($"Cargando Paciente desde SQL: ID:({paciente.Id}) - {paciente.Name} {paciente.LastName}");

						DictPacientes[paciente.Id] = paciente;
					}
				}
			}
		} catch (Exception ex) {
			MessageBox.Show($"Ocurrio un error al leer la tabla SQL de ModelViewPaciente: {ex.Message}", "Error de Database", MessageBoxButton.OK, MessageBoxImage.Error);
			return CrearLasTablasExitosamente();
		}
		return true;
	}
	//------------------------private.LOAD.WindowListarTurnos----------------------//
	private bool SQLCargarTurnosExitosamente() {
		try {
			using (var conexion = new SqlConnection(connectionString)) {
				conexion.Open();
				string consulta = "SELECT * FROM Turno";
				using (var sqlComando = new SqlCommand(consulta, conexion))
				using (var reader = sqlComando.ExecuteReader()) {
					while (reader.Read()) {
						var turno = new ModelViewTurno(
							reader["Id"]?.ToString(),
							reader["PacienteId"]?.ToString(),
							reader["MedicoId"]?.ToString(),
							reader["Fecha"] != DBNull.Value ? Convert.ToDateTime(reader["Fecha"]) : null,
							reader["Hora"].ToString(),
							reader["DuracionMinutos"] != DBNull.Value ? Convert.ToInt32(reader["DuracionMinutos"]) : null
						);
						DictTurnos[turno.Id] = turno;
					}
				}
			}
		} catch (Exception ex) {
			MessageBox.Show($"Ocurrio un error al leer la tabla SQL de ModelViewTurno: {ex.Message}", "Error de Database", MessageBoxButton.OK, MessageBoxImage.Error);
			return CrearLasTablasExitosamente();
		}
		return true;
	}


	//---------------------------private.LOGIN--------------------------//

	private bool AsegurarArchivoAppConfig(string? customConnectionString = null) {
		connectionString = customConnectionString ?? ConfigurationManager.ConnectionStrings["ConexionAClinicaMedica"]?.ConnectionString;
		if (connectionString == null) {
			MessageBox.Show("No se pudo leer la cadena de conexion desde el archivo ''App.config o Clinica.AppWPF.dll.config''. Existe ese archivo?", "Error de configuracion", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		return true;
	}

	public bool CadenaDeConexionEsValida() {
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
			}
			return true;
		} catch (SqlException ex) when (ex.Number == 4060) {
			return CrearLaDatabaseExitosamente();
		} catch (SqlException ex) when (ex.Number == 53 || ex.Number == 40) {
			MessageBox.Show($"Error al conectar con el servidor. \n Mal tipeado, no existe o simplemente no se puede conectar. \n Cadena de conexion: {connectionString}", "Error de servidor", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		} catch (SqlException ex) when (ex.Number == 18456) {
			MessageBox.Show($"Credenciales incorrectas. Verificar usuario y contraseña de Microsoft SQL Server. \n Cadena de conexion: {connectionString}", "Error de login", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		} catch (Exception ex) {
			MessageBox.Show($"Error no esperado. Verificar usuario. \n Cadena de conexion: {connectionString}. \n Mas informacion: \n{ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}


	private bool CrearLaDatabaseExitosamente() {
		if (MessageBox.Show($"Database 'ClinicaMedica'no existe. Desea crearla?\n Se va a crear la tabla como 'master', y despuse se van a hacer las tablas y los inserts como 'ClinicaMedica'.", "Confirmar creación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) {
			return
				EjecutarScriptExitosamente(cadena: connectionString.Replace("Database=ClinicaMedica", "Database=master"), script: "CREATE DATABASE ClinicaMedica;")
				&& EjecutarScriptExitosamente(cadena: connectionString, script: File.ReadAllText(_scriptClinicaMedicaCreaTablasConInserts))
				&& CadenaDeConexionEsValida()
			;
		}
		return false;
	}
	private bool CrearLasTablasExitosamente() {
		if (MessageBox.Show($"Faltan tablas en la base de datos. Desea correr un script para regenerarlas, con algunos inserts?", "Confirmar creación", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) {
			return
				EjecutarScriptExitosamente(cadena: connectionString, script: File.ReadAllText(_scriptClinicaMedicaCreaTablasConInserts))
				&& CadenaDeConexionEsValida()
			;
		}
		return false;
	}

	private bool EjecutarScriptExitosamente(string cadena, string script) {
		try {
			using (SqlConnection connection = new SqlConnection(cadena)) {
				connection.Open();
				// MessageBox.Show($"Conexion establecida: {cadena}", "Confirmar acción");
				using (SqlCommand command = new SqlCommand(script, connection)) {
					command.ExecuteNonQuery();
				}
			}
		} catch (Exception ex) {
			MessageBox.Show($"Error inesperado. {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		return true;
	}



	//------------------------settings----------------------//
	// public override bool EliminarDatabaseExitosamente() {
	// if (MessageBox.Show($"Desea eliminar la Database 'ClinicaMedica'?. Cadena actual: {connectionString}", "Confirmar acción", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) {
	// return (
	// EjecutarScriptExitosamente(cadena: connectionString.Replace("Database=ClinicaMedica", "Database=master"), script: "DROP DATABASE ClinicaMedica;")
	// && CadenaDeConexionEsValida()
	// );
	// }
	// return false;
	// }
	//------------------------Fin.BaseDeDatosSQL----------------------//
}
