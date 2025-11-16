using Clinica.Dominio.TiposDeValor;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Clinica.AppWPF.ModelViews;

public partial class ModelViewHorariosAgrupados : ObservableObject {
	public DayOfWeek DiaSemana { get; }
	public string DiaSemanaNombre => DiaSemana.AEspañol();
	[ObservableProperty] private ObservableCollection<ModelViewHorario> horarios = new();

	public ModelViewHorariosAgrupados(DayOfWeek dia) {
		DiaSemana = dia;
	}
}