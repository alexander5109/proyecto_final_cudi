using System.Windows;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.ViewModels;
//---------------------------------Tablas.WindowListarTurnos-------------------------------//
public partial class WindowModificarTurnoViewModel : ObservableObject {
	[ObservableProperty] private int? id = default;
	[ObservableProperty] private int? pacienteId = default;
	[ObservableProperty] private int? medicoId = default;
	[ObservableProperty] private DateTime? fecha = null;
	[ObservableProperty] private string? hora = null;
	[ObservableProperty] private int? duracionMinutos = null;
	//public WindowModificarMedicoViewModel MedicoRelacionado => App.BaseDeDatos.GetMedicoById(MedicoId ?? throw new Exception("El ID del médico es nulo."));
	//public WindowModificarPacienteViewModel PacienteRelacionado => App.BaseDeDatos.GetPacienteById(PacienteId ?? throw new Exception("El ID del paciente es nulo."));

	public static WindowModificarTurnoViewModel NewEmpty() => new(
		id: default,
		pacienteId: default,
		medicoId: default,
		fecha: null,
		hora: string.Empty,
		duracionMinutos: null
	);

    internal Result<Turno2025> ToDomain() {
        throw new NotImplementedException();
    }

    public WindowModificarTurnoViewModel(
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
