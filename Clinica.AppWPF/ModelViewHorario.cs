using Clinica.Dominio.Tipos;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Clinica.AppWPF.ModelViews;

public partial class ModelViewHorario : ObservableObject {
	[ObservableProperty] private DayOfWeek diaSemana;
	[ObservableProperty] private TimeOnly desde;
	[ObservableProperty] private TimeOnly hasta;
}