
using Clinica.AppWPF.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Ventanas;

public partial class HorarioModificarViewModel : ObservableObject {
	public MedicoDto Medico { get; }

	[ObservableProperty]
	private HorarioMedicoDto horario;

	//public IEnumerable<DayOfWeek> DiaSemanaValues => new HorarioModificarViewModel();

	public bool EsNuevo { get; }

	//public HorarioModificarViewModel(MedicoDto medico, HorarioMedicoDto horario, bool esNuevo) {
	//	Medico = medico;
	//	Horario = horario;
	//	EsNuevo = esNuevo;
	//}
}