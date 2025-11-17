using Clinica.AppWPF.ViewModels;
using Clinica.DataPersistencia.ModelDtos;
using Clinica.DataPersistencia.Repositorios;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
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

	//------------------------public.CREATE.ViewModelMedico----------------------//
	public override bool CreateMedico(Medico2025 instancia, ViewModelMedico instanceDto) {
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
					instanceDto.Id = int.Parse(sqlComando.ExecuteScalar().ToString()); //ahora la instancia creada desde la ventana tiene su propia CodigoInterno
				}
			}
			if (instanceDto.Id == null) {
				throw new Exception("Error al obtener la ID generada para el médico.");
			} else {
				DictMedicos[(int)instanceDto.Id] = instanceDto;
			}
			// MessageBox.Show($"Exito: Se ha creado la instancia de ViewModelMedico: {instancia.Name} {instancia.LastName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
	public override bool CreatePaciente(Paciente2025 instancia, ViewModelPaciente instanceDto) {
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
					instanceDto.Id = int.Parse(sqlComando.ExecuteScalar()?.ToString());    //ahora la instancia creada desde la ventana tiene su propia CodigoInterno
				}
			}
			if (instanceDto.Id == null) {
				throw new Exception("Error al obtener la ID generada para el paciente.");
			}
			DictPacientes[(int) instanceDto.Id] = instanceDto;
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
	public override bool CreateTurno(Turno2025 instanciaValidada, ViewModelTurno instancia) {
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
					instancia.Id = int.Parse(sqlComando.ExecuteScalar().ToString());   //ahora la instancia creada desde la ventana tiene su propia CodigoInterno
				}
			}
			if (instancia.Id is null) {
				throw new Exception("Error al obtener la ID generada para el turno.");
			}
			DictTurnos[(int)instancia.Id] = instancia;
			//MessageBox.Show($"Exito: Se ha creado la instancia de ModelViewTurno con id: {instancia.CodigoInterno} entre {instancia.PacienteId} {instancia.MedicoId} el dia {instancia.Fecha} a las {instancia.Hora}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
	public override List<ViewModelMedico> ReadMedicos() {
		return DictMedicos.Values.ToList();
	}

	public override List<ViewModelPaciente> ReadPacientes() {
		return DictPacientes.Values.ToList();
	}

	public override List<ViewModelTurno> ReadTurnos() {
		return DictTurnos.Values.ToList();
	}











	//------------------------public.UPDATE.ViewModelMedico----------------------//
	public override bool UpdateMedico(Medico2025 instancia, int? instanceId) {
		if (instanceId == null) {
			return false;
		}
		string query = "UPDATE Medico SET Name = @Name, LastName = @LastName, Dni = @Dni, Provincia = @Provincia, Domicilio = @Domicilio, Localidad = @Localidad, Especialidad = @Especialidad, Telefono = @Telefono, Guardia = @Guardia, FechaIngreso = @FechaIngreso, SueldoMinimoGarantizado = @SueldoMinimoGarantizado WHERE CodigoInterno = @CodigoInterno";
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
					sqlComando.Parameters.AddWithValue("@CodigoInterno", instanceId);
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
	public override bool UpdatePaciente(Paciente2025 instancia, int? instanceId) {
		if (instanceId == null) { return false; }
		string query = "UPDATE Paciente SET Dni = @Dni, Name = @Name, LastName = @LastName, FechaIngreso = @FechaIngreso, Email = @Email, Telefono = @Telefono, FechaNacimiento = @FechaNacimiento, Domicilio = @Domicilio, Localidad = @Localidad, Provincia = @Provincia WHERE CodigoInterno = @CodigoInterno";
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
					sqlComando.Parameters.AddWithValue("@CodigoInterno", instanceId);
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
	public override bool UpdateTurno(Turno2025 instanciaValidada, ViewModelTurno instancia) {
		string query = "UPDATE Turno SET PacienteId = @PacienteId, MedicoId = @MedicoId, Fecha = @Fecha, Hora = @Hora WHERE CodigoInterno = @CodigoInterno";
		// string query = "UPDATE ModelViewTurno SET PacienteId = @PacienteId, MedicoId = @MedicoId WHERE CodigoInterno = @CodigoInterno";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@PacienteId", instancia.PacienteId);
					sqlComando.Parameters.AddWithValue("@MedicoId", instancia.MedicoId);
					sqlComando.Parameters.AddWithValue("@Fecha", instancia.Fecha);
					sqlComando.Parameters.AddWithValue("@Hora", instancia.Hora);
					sqlComando.Parameters.AddWithValue("@CodigoInterno", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			//MessageBox.Show($"Exito: Se han actualizado los datos del turno con id: {instancia.CodigoInterno}. Ahora entre {instancia.PacienteId} {instancia.MedicoId} el dia {instancia.Fecha} a las {instancia.Hora}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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








	//------------------------public.DELETE.ViewModelMedico----------------------//
	public override bool DeleteMedico(ViewModelMedico instancia) {
		string query = "DELETE FROM Medico WHERE CodigoInterno = @CodigoInterno";

		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@CodigoInterno", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			if (instancia.Id == null) {
				throw new Exception("Error al obtener la ID del médico para eliminar.");
			}
			DictMedicos.Remove((int) instancia.Id);
			// MessageBox.Show($"Exito: Se ha eliminado el medicoView con id: {instancia.CodigoInterno} de la Base de Datos SQL", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
	public override bool DeletePaciente(ViewModelPaciente instancia) {
		if (instancia.Id == null) {
			throw new Exception("Error al obtener la ID del paciente para eliminar.");
		}
		string query = "DELETE FROM Paciente WHERE CodigoInterno = @CodigoInterno";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@CodigoInterno", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			DictPacientes.Remove((int) instancia.Id);
			// MessageBox.Show($"Exito: Se ha eliminado el paciente con id: {instancia.CodigoInterno} de la Base de Datos SQL", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
	public override bool DeleteTurno(ViewModelTurno instancia) {
		if (instancia.Id == null) {
			throw new Exception("Error al obtener la ID del turno para eliminar.");
		}
		string query = "DELETE FROM Turno WHERE CodigoInterno = @CodigoInterno";
		try {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand sqlComando = new SqlCommand(query, connection)) {
					sqlComando.Parameters.AddWithValue("@CodigoInterno", instancia.Id);
					sqlComando.ExecuteNonQuery();
				}
			}
			DictTurnos.Remove((int)instancia.Id);
			// MessageBox.Show($"Exito: Se ha eliminado el turno con id: {instancia.CodigoInterno} de la Base de Datos SQL", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
		//var dictMedicos = new Dictionary<int, ViewModelMedico>();
		foreach (MedicoDto medico in MedicosRepositoriosOld.ReadTodos(connectionString)) {
			DictMedicos[medico.Id] = medico.ToViewModel();
			// MessageBox.Show($"Cargando Medico desde SQL: ID:({medico.Id}) - {medico.Name} {medico.LastName} {medico.Horarios.ToString()}");

		}
		return true;
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
                        ViewModelPaciente paciente = new ViewModelPaciente(
							Convert.ToInt32(reader["Id"]),
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
						//MessageBox.Show($"Cargando Paciente desde SQL: ID:({paciente.CodigoInterno}) - {paciente.Name} {paciente.LastName}");
						if (paciente.Id == null) {
							throw new Exception("Error al obtener la ID del paciente desde la base de datos.");
						} else {
							DictPacientes[(int) paciente.Id] = paciente;
						}
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
                        ViewModelTurno turno = new(
							int.Parse(reader["Id"].ToString()),
							int.Parse(reader["PacienteId"].ToString()),
							int.Parse(reader["MedicoId"].ToString()),
							reader["Fecha"] != DBNull.Value ? Convert.ToDateTime(reader["Fecha"]) : null,
							reader["Hora"].ToString(),
							reader["DuracionMinutos"] != DBNull.Value ? Convert.ToInt32(reader["DuracionMinutos"]) : null
						);
						if (turno != null) {
							DictTurnos[(int) turno.Id] = turno;
						}
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
