using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Clinica.AppWPF.CommonViewModels;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;
using Clinica.Shared.ApiDtos;
using static Clinica.AppWPF.CommonViewModels.CommonEnumsToViewModel;
using static Clinica.Shared.ApiDtos.HorarioDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioAdministrativo;

public class DialogoModificarHorariosVM : INotifyPropertyChanged {

	// ================================================================
	// CONSTRUCTORES Y CONTEXTO
	// ================================================================
	public DialogoModificarHorariosVM(MedicoDbModel medico) {
		ActiveMedicoModel = medico;

		_ = CargarHorariosAsync(medico.Id);
	}



	// ================================================================
	// COLLECTIONS
	// ================================================================

	public ObservableCollection<ViewModelHorarioAgrupado> HorariosAgrupados { get; } = [];


	// ================================================================
	// REGLAS
	// ================================================================

	private bool EstaCreando => Id is null;
	private bool EstaEditando => Id is not null;
	public bool PuedeEliminar => EstaEditando;
	public bool PuedeGuardarCambios => TieneCambios;
	public bool PuedeEditarHorario => true; // allow editing/adding/deleting horarios in the dialog





	// ================================================================
	// CONTEXTO DE TURNO
	// ================================================================


	public MedicoDbModel ActiveMedicoModel { get; private set; }
	public string? ActiveMedicoEspecialidad => ActiveMedicoModel?.EspecialidadCodigo.ToString();
	public string? ActiveMedicoNombreCompleto => $"{ActiveMedicoModel?.Nombre} {ActiveMedicoModel?.Apellido}";





	// -----------------------------
	// DETECTAR CAMBIOS
	// -----------------------------

	public bool TieneCambios => true;

	// -----------------------------
	// METHODS
	// -----------------------------

	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		if (!PuedeGuardarCambios)
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No hay cambios para guardar.", MessageBoxImage.Information));

		return await ToDomain(fechaIngreso: DateTime.Now)
			.Bind(medico => EstaEditando
				? GuardarEdicionAsync(medico)
				: GuardarCreacionAsync(medico)
			);
	}
	private async Task<ResultWpf<UnitWpf>> GuardarEdicionAsync(Medico2025 medico) {
		if (Id is MedicoId idNotNull) {
			var agg = new Medico2025Agg(idNotNull, medico);
			// Build horarios DTOs from HorariosAgrupados
			List<HorarioDto> horarios = [];
			foreach (var grupo in HorariosAgrupados) {
				foreach (var h in grupo.Horarios) {
					horarios.Add(new HorarioDto {
						MedicoId = idNotNull,
						DiaSemana = h.DiaSemana,
						HoraDesde = h.HoraDesde.ToTimeSpan(),
						HoraHasta = h.HoraHasta.ToTimeSpan(),
						VigenteDesde = h.VigenteDesde,
						VigenteHasta = h.VigenteHasta // now non-nullable in viewmodel
					});
				}
			}

			return await App.Repositorio.UpdateMedicoWhereIdWithHorarios(idNotNull, medico, horarios);
		} else {
			return new ResultWpf<UnitWpf>.Error(new ErrorInfo("No se puede guardar, la entidad no tiene Id.", MessageBoxImage.Information));
		}
	}
	private async Task<ResultWpf<UnitWpf>> GuardarCreacionAsync(Medico2025 medico) {
		return (await App.Repositorio.InsertMedicoReturnId(medico))
			.MatchTo(
				ok => {
					Id = ok;
					OnPropertyChanged(nameof(Id));
					OnPropertyChanged(nameof(EstaCreando));
					OnPropertyChanged(nameof(EstaEditando));
					OnPropertyChanged(nameof(PuedeEliminar));
					return new ResultWpf<UnitWpf>.Ok(UnitWpf.Valor);
				},
				error => new ResultWpf<UnitWpf>.Error(error)
			);
	}


	public async Task CargarHorariosAsync(MedicoId idGood) {
		HorariosAgrupados.Clear();
		//if (Id is not MedicoId idGood) {
		//	MessageBox.Show("why is medicoid null?");
		//	return;
		//}
		IReadOnlyList<HorarioDbModel>? horarios = await App.Repositorio.SelectHorariosWhereMedicoId(idGood);
		if (horarios == null) {
			MessageBox.Show("why is horarios null?");
			return;
		}

		IOrderedEnumerable<IGrouping<DayOfWeek, HorarioDbModel>> grupos = horarios
			.GroupBy(h => h.DiaSemana)
			.OrderBy(g => g.Key);


		foreach (IGrouping<DayOfWeek, HorarioDbModel>? grupo in grupos) {
			HorariosAgrupados.Add(
				new ViewModelHorarioAgrupado(
					grupo.Key,
					[.. grupo]
				)
			);
		}
	}


	// ================================================================
	// INFRAESTRUCTURA
	// ================================================================

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new(prop));

}



// ================================================================
// MINIVIEWMODELS
// ================================================================


public class ViewModelHorarioAgrupado(DayOfWeek dia, List<HorarioDbModel> horarios) {
	public DayOfWeek DiaSemana { get; } = dia;
	public string DiaSemanaNombre { get; } = dia.ATexto();
	//public string DiaSemanaNombre { get; } = CultureInfo.GetCultureInfo("es-AR").DateTimeFormat.DayNames[dia];
	public ObservableCollection<HorarioMedicoViewModel> Horarios { get; } = new ObservableCollection<HorarioMedicoViewModel>(
			horarios.Select(h => new HorarioMedicoViewModel(h))
		);
}

public class HorarioMedicoViewModel : INotifyPropertyChanged {

	public HorarioMedicoViewModel(HorarioDbModel h) {
		Model = h;
		_dia = h.DiaSemana;
		_horaDesde = TimeOnly.FromTimeSpan(h.HoraDesde);
		_horaHasta = TimeOnly.FromTimeSpan(h.HoraHasta);
		VigenteDesde = h.VigenteDesde;
		VigenteHasta = h.VigenteHasta ?? DateTime.MaxValue; // coalesce nullable
	}

	public HorarioMedicoViewModel(DayOfWeek dia, TimeOnly desde, TimeOnly hasta) {
		Model = new HorarioDbModel();
		_dia = dia;
		_horaDesde = desde;
		_horaHasta = hasta;
		VigenteDesde = DateTime.Today;
		VigenteHasta = DateTime.MaxValue;
	}
	public HorarioDbModel Model { get; private set; }

	private DayOfWeek _dia;
	public DayOfWeek DiaSemana {
		get => _dia;
		set { _dia = value; OnPropertyChanged(nameof(DiaSemana)); OnPropertyChanged(nameof(Desde)); OnPropertyChanged(nameof(Hasta)); }
	}

	private TimeOnly _horaDesde;
	public TimeOnly HoraDesde {
		get => _horaDesde;
		set { _horaDesde = value; OnPropertyChanged(nameof(HoraDesde)); OnPropertyChanged(nameof(Desde)); }
	}
	private TimeOnly _horaHasta;
	public TimeOnly HoraHasta {
		get => _horaHasta;
		set { _horaHasta = value; OnPropertyChanged(nameof(HoraHasta)); OnPropertyChanged(nameof(Hasta)); }
	}

	public DateTime VigenteDesde { get; set; }
	public DateTime VigenteHasta { get; set; } // Changed to non-nullable

	public string Desde => HoraDesde.ToString("hh\\:mm");
	public string Hasta => HoraHasta.ToString("hh\\:mm");

	public HorarioDto ToDto(MedicoId medicoId) => new HorarioDto {
		MedicoId = medicoId,
		DiaSemana = DiaSemana,
		HoraDesde = HoraDesde.ToTimeSpan(),
		HoraHasta = HoraHasta.ToTimeSpan(),
		VigenteDesde = VigenteDesde,
		VigenteHasta = VigenteHasta
	};

	public event PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new(name));
}
