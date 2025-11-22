using Clinica.Dominio.Entidades;
using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Clinica.AppWPF.ViewModels;
//---------------------------------Tablas.WindowListarTurnos-------------------------------//
public partial class ViewModelTurno : ObservableObject {
	[ObservableProperty] private int? id = default;
	[ObservableProperty] private int? pacienteId = default;
	[ObservableProperty] private int? medicoId = default;
	[ObservableProperty] private DateTime? fecha = null;
	[ObservableProperty] private string? hora = null;
	[ObservableProperty] private int? duracionMinutos = null;
	public ViewModelMedico MedicoRelacionado {
		get {
			try {
				if (MedicoId is null)
					throw new Exception("El ID del médico es nulo.");
				return App.BaseDeDatos.DictMedicos[(int) MedicoId];
			} catch (Exception ex) {
				MessageBox.Show(
					$"Error al obtener el médico con ID '{MedicoId}':\n{ex.Message}",
					"Error de acceso a datos",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				// Devolvemos un objeto vacío para evitar NullReference
				return ViewModelMedico.NewEmpty();
			}
		}
	}

	public ViewModelPaciente PacienteRelacionado {
		get {
			try {
				if (PacienteId is null)
					throw new Exception("El ID del paciente es nulo.");
				return App.BaseDeDatos.DictPacientes[(int)PacienteId];
			} catch (Exception ex) {
				MessageBox.Show(
					$"Error al obtener el paciente con ID '{PacienteId}':\n{ex.Message}",
					"Error de acceso a datos",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				return ViewModelPaciente.NewEmpty();
			}
		}
	}



	public static ViewModelTurno NewEmpty() => new(
		id: default,
		pacienteId: default,
		medicoId: default,
		fecha: null,
		hora: string.Empty,
		duracionMinutos: null
	);

	public ViewModelTurno(
		int? id,
		int? pacienteId,
		int? medicoId,
		DateTime? fecha,
		string? hora,
		int? duracionMinutos
	) {
		Id = id;
		PacienteId = pacienteId;
		MedicoId = medicoId;
		Fecha = fecha;
		Hora = hora ?? string.Empty;
		DuracionMinutos = duracionMinutos;
	}

}

public static class ViewModelTurnoExtensions {

	static public Result<Turno2025> ToDomain(this ViewModelTurno viewModelTurno) {
		// 🕒 Validar la fecha
		if (viewModelTurno.Fecha is null)
			return new Result<Turno2025>.Error("Debe seleccionar una fecha para el turno.");
		// 🕧 Intentar parsear la hora
		if (string.IsNullOrWhiteSpace(viewModelTurno.Hora))
			return new Result<Turno2025>.Error("Debe especificar una hora para el turno.");

		if (!TimeOnly.TryParse(viewModelTurno.Hora, out var horaParte))
			return new Result<Turno2025>.Error($"La hora '{viewModelTurno.Hora}' no tiene un formato válido (use HH:mm).");

		// ✅ Combinar fecha y hora
		DateOnly fechaParte = DateOnly.FromDateTime(viewModelTurno.Fecha.Value);
		DateTime fechaYHora = fechaParte.ToDateTime(horaParte);

		// Obtener resultados de dominio de modelos relacionados
		var medicoDomainR = viewModelTurno.MedicoRelacionado.ToDomain();
		var pacienteDomainR = viewModelTurno.PacienteRelacionado.ToDomain();

		if (medicoDomainR is Result<Medico2025>.Error medErr)
			return new Result<Turno2025>.Error($"Error al crear médico de dominio: {medErr.Mensaje}");
		if (pacienteDomainR is Result<Paciente2025>.Error pacErr)
			return new Result<Turno2025>.Error($"Error al crear paciente de dominio: {pacErr.Mensaje}");

		var medicoDomain = ((Result<Medico2025>.Ok)medicoDomainR).Valor;
		var pacienteDomain = ((Result<Paciente2025>.Ok)pacienteDomainR).Valor;

		// Construir especialidad a partir del médico (usamos rama por defecto si se requiere)
		var especialidadResult = EspecialidadMedica2025.CrearPorCodigoInterno(viewModelTurno.MedicoRelacionado.EspecialidadCodigoInterno);

		if (especialidadResult is Result<EspecialidadMedica2025>.Error espErr)
			return new Result<Turno2025>.Error($"Error al crear especialidad: {espErr.Mensaje}");

		return Turno2025.Programar(
			new Result<Medico2025>.Ok(medicoDomain),
			new Result<Paciente2025>.Ok(pacienteDomain),
			especialidadResult,
			fechaYHora
		);
	}
}