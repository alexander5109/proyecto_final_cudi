using Clinica.AppWPF.ModelViews;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Windows;

namespace Clinica.AppWPF.Entidades;
//---------------------------------Tablas.Turnos-------------------------------//
public partial class TurnoView : ObservableObject {
	[ObservableProperty] private string id = string.Empty;
	[ObservableProperty] private string pacienteId = string.Empty;
	[ObservableProperty] private string medicoId = string.Empty;
	[ObservableProperty] private DateTime? fecha = null;
	[ObservableProperty] private string? hora = null;
	[ObservableProperty] private int? duracionMinutos = null;
	public MedicoView MedicoRelacionado {
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
				return MedicoView.NewEmpty();
			}
		}
	}

	public PacienteView PacienteRelacionado {
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
				return PacienteView.NewEmpty();
			}
		}
	}



	public static TurnoView NewEmpty() => new(
		id: string.Empty,
		pacienteId: string.Empty,
		medicoId: string.Empty,
		fecha: null,
		hora: string.Empty,
		duracionMinutos: null
	);

	public TurnoView(
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

		// 🏥 Crear el turno de dominio
		return Turno2025.Crear(
			MedicoRelacionado.ToDomain(),
			PacienteRelacionado.ToDomain(),
			fechaYHora,
			DuracionMinutos
		);
	}

}
