using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Clinica.AppWPF.ModelViews;
//---------------------------------Tablas.WindowListarTurnos-------------------------------//
public partial class ModelViewTurno : ObservableObject {
	[ObservableProperty] private string id = string.Empty;
	[ObservableProperty] private string pacienteId = string.Empty;
	[ObservableProperty] private string medicoId = string.Empty;
	[ObservableProperty] private DateTime? fecha = null;
	[ObservableProperty] private string? hora = null;
	[ObservableProperty] private int? duracionMinutos = null;
	public ModelViewMedico MedicoRelacionado {
		get {
			try {
				return App.BaseDeDatos.DictMedicos[MedicoId];
			} catch (Exception ex) {
				MessageBox.Show(
					$"Error al obtener el médico con ID '{MedicoId}':\n{ex.Message}",
					"Error de acceso a datos",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				// Devolvemos un objeto vacío para evitar NullReference
				return ModelViewMedico.NewEmpty();
			}
		}
	}

	public ModelViewPaciente PacienteRelacionado {
		get {
			try {
				return App.BaseDeDatos.DictPacientes[PacienteId];
			} catch (Exception ex) {
				MessageBox.Show(
					$"Error al obtener el paciente con ID '{PacienteId}':\n{ex.Message}",
					"Error de acceso a datos",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				return ModelViewPaciente.NewEmpty();
			}
		}
	}



	public static ModelViewTurno NewEmpty() => new(
		id: string.Empty,
		pacienteId: string.Empty,
		medicoId: string.Empty,
		fecha: null,
		hora: string.Empty,
		duracionMinutos: null
	);

	public ModelViewTurno(
		string? id,
		string? pacienteId,
		string? medicoId,
		DateTime? fecha,
		string? hora,
		int? duracionMinutos
	) {
		Id = id ?? string.Empty;
		PacienteId = pacienteId ?? string.Empty;
		MedicoId = medicoId ?? string.Empty;
		Fecha = fecha;
		Hora = hora ?? string.Empty;
		DuracionMinutos = duracionMinutos;
	}

	public Result<Turno2025> ToDomain() {
		// 🕒 Validar la fecha
		if (Fecha is null)
			return new Result<Turno2025>.Error("Debe seleccionar una fecha para el turno.");
		// 🕧 Intentar parsear la hora
		if (string.IsNullOrWhiteSpace(Hora))
			return new Result<Turno2025>.Error("Debe especificar una hora para el turno.");

		if (!TimeOnly.TryParse(Hora, out var horaParte))
			return new Result<Turno2025>.Error($"La hora '{Hora}' no tiene un formato válido (use HH:mm).");

		// ✅ Combinar fecha y hora
		DateOnly fechaParte = DateOnly.FromDateTime(Fecha.Value);
		DateTime fechaYHora = fechaParte.ToDateTime(horaParte);

		// Obtener resultados de dominio de modelos relacionados
		var medicoDomainR = MedicoRelacionado.ToDomain();
		var pacienteDomainR = PacienteRelacionado.ToDomain();

		if (medicoDomainR is Result<Medico2025>.Error medErr)
			return new Result<Turno2025>.Error($"Error al crear médico de dominio: {medErr.Mensaje}");
		if (pacienteDomainR is Result<Paciente2025>.Error pacErr)
			return new Result<Turno2025>.Error($"Error al crear paciente de dominio: {pacErr.Mensaje}");

		var medicoDomain = ((Result<Medico2025>.Ok)medicoDomainR).Valor;
		var pacienteDomain = ((Result<Paciente2025>.Ok)pacienteDomainR).Valor;

		// Construir especialidad a partir del médico (usamos rama por defecto si se requiere)
		var especialidadResult = MedicoEspecialidad2025.Crear(medicoDomain.NombreCompleto.Nombre, MedicoEspecialidad2025.RamasDisponibles.First());

		if (especialidadResult is Result<MedicoEspecialidad2025>.Error espErr)
			return new Result<Turno2025>.Error($"Error al crear especialidad: {espErr.Mensaje}");

		return Turno2025.Crear(
			new Result<Medico2025>.Ok(medicoDomain),
			new Result<Paciente2025>.Ok(pacienteDomain),
			especialidadResult,
			fechaYHora
		);
	}

}
