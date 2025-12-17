using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico;





public sealed class MedicoAtencionDelDiaVM(MedicoId2025 CurrentMedicoId) : INotifyPropertyChanged {
	// ==========================================================
	// BOTONES: SELECTED ITEMS
	// ==========================================================.




	private DateTime _selectedFecha = DateTime.Now;
	public DateTime SelectedFecha {
		get => _selectedFecha;
		set {
			if (_selectedFecha == value) return;
			_selectedFecha = value;
			OnPropertyChanged(nameof(SelectedFecha));
			AplicarFiltroPorFecha(); // 🔥 CLAVE
		}
	}


	private PacienteDbModel? _selectedPaciente;
	public PacienteDbModel? SelectedPaciente {
		get => _selectedPaciente;
		set {
			if (_selectedPaciente != value) {
				_selectedPaciente = value;
				OnPropertyChanged(nameof(SelectedPaciente));
				OnPropertyChanged(nameof(HayPacienteSeleccionado));
				OnSelectedPacienteChanged();
			}
		}
	}


	private TurnoDeHoyVM? _selectedTurno;
	public TurnoDeHoyVM? SelectedTurno {
		get => _selectedTurno;
		set {
			if (_selectedTurno != value) {
				_selectedTurno = value;
				OnPropertyChanged(nameof(SelectedTurno));
				OnPropertyChanged(nameof(PuedeDiagnosticarYConfirmar));
				ActualizarPacienteDesdeTurno();
			}
		}
	}
	private string? _diagnosticoText;
	public string? Observaciones {
		get => _diagnosticoText;
		set {
			if (_diagnosticoText != value) {
				_diagnosticoText = value;
				OnPropertyChanged(nameof(Observaciones));
			}
		}
	}

	// ================================================================
	// COLECCIONES
	// ================================================================

	public ObservableCollection<AtencionPreviaVM> AtencionesViewModelList { get; } = [];
	public ObservableCollection<TurnoDeHoyVM> TurnosList { get; } = [];
	private List<TurnoDbModel> _todosLosTurnos = [];
	private List<AtencionDbModel> _atencionesDelDia = [];


	// ================================================================
	// METHODS.PUBLIC
	// ================================================================

	internal async Task RefrescarTodoAsync() {

		await App.Repositorio.Pacientes.RefreshCache();
		await App.Repositorio.Medicos.RefreshCache();

		_todosLosTurnos =
			await App.Repositorio.Turnos
				.SelectTurnosWhereMedicoId(CurrentMedicoId);

		_atencionesDelDia =
			await App.Repositorio.Atenciones
				.SelectAtencionesWhereMedicoId(CurrentMedicoId);

		AplicarFiltroPorFecha();
	}


	internal async Task PostConfirmarDiagnosticoAsync() {
		Observaciones = null;
		await CargarAtencionesDePacienteSeleccionado();
		await RefrescarMisTurnosAsync();
		OnPropertyChanged(nameof(PuedeDiagnosticarYConfirmar));
	}

	private void RestaurarSeleccionTurnoSiExiste(TurnoId2025 turnoId) {
		SelectedTurno = TurnosList.FirstOrDefault(t => t.Model.Id == turnoId);
	}


	private void ActualizarPacienteDesdeTurno() {
		if (SelectedTurno == null) {
			SelectedPaciente = null;
			return;
		}

		SelectedPaciente = App.Repositorio.Pacientes.GetFromCachePacienteWhereId(SelectedTurno.Model.PacienteId);
		OnPropertyChanged(nameof(PuedeDiagnosticarYConfirmar));
	}

	private string CalcularEdad(DateTime fechaNacimiento) =>
		(DateTime.Today.Year - fechaNacimiento.Year - (DateTime.Today < fechaNacimiento.AddYears(DateTime.Today.Year - fechaNacimiento.Year) ? 1 : 0)).ToString();

	// ================================================================
	// METHODS.ASYNC
	// ================================================================

	private void AplicarFiltroPorFecha() {

		TurnoId2025? turnoSeleccionadoId = SelectedTurno?.Model.Id;

		TurnosList.Clear();

		var turnosAtendidos = _atencionesDelDia
			.Select(a => a.TurnoId)
			.ToHashSet();

		var turnosDelDia = _todosLosTurnos
			.Where(t => t.FechaHoraAsignadaDesde.Date == SelectedFecha.Date)
			.OrderBy(t => t.FechaHoraAsignadaDesde);

		foreach (var turno in turnosDelDia) {

			var paciente = App.Repositorio.Pacientes
				.GetFromCachePacienteWhereId(turno.PacienteId);

			if (paciente is null)
				continue;

			bool esTurnoConcretado =
				turno.OutcomeEstado == TurnoEstadoEnum.Concretado;

			bool tieneAtencion =
				turnosAtendidos.Contains(turno.Id);

			TurnosList.Add(new TurnoDeHoyVM(
				model: turno,
				pacienteNombreApellido: $"{paciente.Nombre} {paciente.Apellido}",
				pacienteEdad: CalcularEdad(paciente.FechaNacimiento),
				esTurnoConcretado: esTurnoConcretado,
				tieneAtencion: tieneAtencion
			));
		}

		if (turnoSeleccionadoId is not null)
			SelectedTurno = TurnosList.FirstOrDefault(t => t.Model.Id == turnoSeleccionadoId);

		OnPropertyChanged(nameof(PuedeDiagnosticarYConfirmar));
	}



	public async Task<ResultWpf<UnitWpf>> ConfirmarDiagnosticoAsync() {

		if (!PuedeDiagnosticarYConfirmar)
			return new ResultWpf<UnitWpf>.Error(
				new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information)
			);


		//MessageBox.Show($"|SelectedPaciente?.Nombre: {SelectedPaciente?.Nombre} \n| SelectedTurno?.Hora: {SelectedTurno?.Model.FechaHoraAsignadaDesde}| SelectedTurno?.EsTurnoConcretado: {SelectedTurno?.EsTurnoConcretado}| ObservacionActual: {Observaciones}|");
		if (SelectedPaciente == null || SelectedTurno == null || string.IsNullOrWhiteSpace(Observaciones)) {
			return new ResultWpf<UnitWpf>.Error(
				new ErrorInfo("No hay nada seleccionado.", MessageBoxImage.Information)
			);
		}
		// Solo permitir si el turno está concretado
		if (!SelectedTurno.EsTurnoConcretado)
			return new ResultWpf<UnitWpf>.Error(
				new ErrorInfo("El turno ya esta concretado.", MessageBoxImage.Information)
			);

		return await App.Repositorio.Atenciones.AgendarAtencionConDiagnostico(new AtencionDto(
			SelectedTurno.Model.Id.Valor,
			SelectedPaciente.Id.Valor,
			CurrentMedicoId.Valor,
			Observaciones
			)
		);
	}




	private async void OnSelectedPacienteChanged() {
		await CargarAtencionesDePacienteSeleccionado();
	}


	internal async Task CargaInicial() {

		await RefrescarMisTurnosAsync();
	}


	private async Task RefrescarMisTurnosAsync() {

		// 1. Cargar caches necesarios
		await App.Repositorio.Pacientes.RefreshCache();
		await App.Repositorio.Medicos.RefreshCache();

		// 2. Traer TODOS mis turnos
		List<TurnoDbModel> turnos = await App.Repositorio.Turnos.SelectTurnosWhereMedicoId(CurrentMedicoId);

		_todosLosTurnos = turnos;

		// 3. Traer TODAS las atenciones asociadas a MIS turnos
		//    (sirve solo para saber si el turno ya fue atendido)
		List<AtencionDbModel> atencionesDelMedico = await App.Repositorio.Atenciones.SelectAtencionesWhereMedicoId(CurrentMedicoId);

		var turnosAtendidos = atencionesDelMedico
			.Select(a => a.TurnoId)
			.ToHashSet();

		// 4. Reconstruir la lista
		TurnosList.Clear();

		foreach (TurnoDbModel? turno in turnos.OrderBy(t => t.FechaHoraAsignadaDesde)) {

			PacienteDbModel? paciente = App.Repositorio.Pacientes
				.GetFromCachePacienteWhereId(turno.PacienteId);

			if (paciente is null) {
				// Esto ya es inconsistencia de datos
				MessageBox.Show($"Paciente inexistente para turno {turno.Id.Valor}");
				continue;
			}

			bool esTurnoConcretado =
				turno.OutcomeEstado == TurnoEstadoEnum.Concretado;

			bool tieneAtencion =
				turnosAtendidos.Contains(turno.Id);

			TurnosList.Add(new TurnoDeHoyVM(
				model: turno,
				pacienteNombreApellido: $"{paciente.Nombre} {paciente.Apellido}",
				pacienteEdad: CalcularEdad(paciente.FechaNacimiento),
				esTurnoConcretado: esTurnoConcretado,
				tieneAtencion: tieneAtencion
			));
		}
	}



	internal async Task CargarAtencionesDePacienteSeleccionado() {

		AtencionesViewModelList.Clear();

		if (SelectedPaciente is null)
			return;

		// 1. Especialidad del médico actual (desde cache)
		EspecialidadEnum? especialidadActual =
			App.Repositorio.Medicos.GetFromCacheMedicoWhereId(CurrentMedicoId)?.EspecialidadCodigo;

		if (especialidadActual is null)
			return;

		// 2. Traer TODAS las atenciones del paciente
		List<AtencionDbModel>? atencionesPaciente =
			await App.Repositorio.Atenciones
				.SelectAtencionesWherePacienteId(SelectedPaciente.Id);

		if (atencionesPaciente is null || atencionesPaciente.Count == 0)
			return;

		// 3. Filtrar por misma especialidad
		IOrderedEnumerable<AtencionDbModel> atencionesFiltradas = atencionesPaciente
			.Where(a => {
				MedicoDbModel? medico = App.Repositorio.Medicos.GetFromCacheMedicoWhereId(a.MedicoId);

				return medico?.EspecialidadCodigo == especialidadActual;
			})
			.OrderByDescending(a => a.Fecha);

		// 4. Construir VMs
		foreach (AtencionDbModel? atencion in atencionesFiltradas) {

			string medicoNombreCompleto =
				App.Repositorio.Medicos
					.GetFromCacheMedicoDisplayWhereId(atencion.MedicoId)
				?? "Médico desconocido";

			AtencionesViewModelList.Add(new AtencionPreviaVM(
				Fecha: atencion.Fecha.ToString("yyyy-MM-dd HH:mm"),
				MedicoNombreApellido: medicoNombreCompleto,
				Observaciones: atencion.Observaciones
			));
		}
	}



	// ================================================================
	// REGLAS
	// ================================================================
	public bool PuedeDiagnosticarYConfirmar =>
		SelectedTurno?.PuedeDiagnosticar == true;

	public bool HayPacienteSeleccionado => SelectedPaciente is not null;


	// ================================================================
	// INFRASTRUCTURE
	// ================================================================

	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



	public event PropertyChangedEventHandler? PropertyChanged;
}



// ================================================================
// MINIVIEWMODELS
// ================================================================

public sealed class TurnoDeHoyVM : INotifyPropertyChanged {
	public TurnoDbModel Model { get; }
	public string PacienteNombreApellido { get; }
	public string PacienteEdad { get; }
	public bool EsTurnoConcretado { get; }
	public bool PuedeDiagnosticar => EsTurnoConcretado && !TieneAtencion;

	public bool TieneAtencion { get; }

	public string FueAtendido => TieneAtencion ? "✔" : "";

	//public string FueAtendido => TieneAtencion ? "✔" : "";
	public TurnoDeHoyVM(
		TurnoDbModel model,
		string pacienteNombreApellido,
		string pacienteEdad,
		bool esTurnoConcretado,
		bool tieneAtencion
	) {
		Model = model;
		PacienteNombreApellido = pacienteNombreApellido;
		PacienteEdad = pacienteEdad;
		EsTurnoConcretado = esTurnoConcretado;
		TieneAtencion = tieneAtencion;
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public record AtencionPreviaVM(
	string Fecha,
	string MedicoNombreApellido,
	string Observaciones
);