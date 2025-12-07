using System.ComponentModel;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.ApiDtos;


namespace Clinica.AppWPF.UsuarioSecretaria;

public sealed class TurnoVM {
	public TurnoId Id { get; }
	public string Fecha { get; }
	public string HoraDesde { get; }
	public string HoraHasta { get; }
	public TurnoOutcomeEstadoCodigo2025 EstadoCodigo { get; }
	public string? Comentario { get; set; }

	public TurnoVM(TurnoDto dto) {
		Id = dto.Id;
		Fecha = dto.FechaHoraAsignadaDesde.AFechaArgentina();
		HoraDesde = dto.FechaHoraAsignadaDesde.AHorasArgentina();
		HoraHasta = dto.FechaHoraAsignadaHasta.AHorasArgentina();
		EstadoCodigo = dto.OutcomeEstado;
		Comentario = dto.OutcomeComentario;
	}
}

public sealed class SecretariaGeneralViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;

	private PacienteDto? _pacienteSeleccionado;
	private TurnoVM? _turnoSeleccionado;

	public PacienteDto? SelectedPaciente {
		get => _pacienteSeleccionado;
		set {
			if (_pacienteSeleccionado != value) {
				_pacienteSeleccionado = value;
				OnPropertyChanged(nameof(SelectedPaciente));
				OnPropertyChanged(nameof(HayPacienteSeleccionado));
			}
		}
	}
	public TurnoVM? TurnoSeleccionado {
		get => _turnoSeleccionado;
		set {
			if (_turnoSeleccionado != value) {
				_turnoSeleccionado = value;
				OnPropertyChanged(nameof(TurnoSeleccionado));
				OnPropertyChanged(nameof(HayTurnoSeleccionado));
				OnPropertyChanged(nameof(BotonesEstadoHabilitados));
				OnPropertyChanged(nameof(ComentarioObligatorio));
			}
		}
	}
	public bool HayPacienteSeleccionado => SelectedPaciente is not null;
	public bool HayTurnoSeleccionado => TurnoSeleccionado is not null;
	public bool BotonesEstadoHabilitados =>
		TurnoSeleccionado?.EstadoCodigo == TurnoOutcomeEstadoCodigo2025.Programado;
	public bool ComentarioObligatorio { get; private set; }

	public void IndicarAccionRequiereComentario(bool requiere) {
		ComentarioObligatorio = requiere;
		OnPropertyChanged(nameof(ComentarioObligatorio));
	}
	private void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
