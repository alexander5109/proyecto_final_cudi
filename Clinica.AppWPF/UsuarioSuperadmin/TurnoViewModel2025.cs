using Clinica.Dominio.Entidades;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clinica.AppWPF.Dtos;
//---------------------------------Tablas.WindowListarTurnos-------------------------------//
public partial class TurnoViewModel2025 : ObservableObject {
	[ObservableProperty] private int? id = default;
	[ObservableProperty] private int? pacienteId = default;
	[ObservableProperty] private int? medicoId = default;
	[ObservableProperty] private DateTime? fecha = null;
	[ObservableProperty] private string? hora = null;
	[ObservableProperty] private int? duracionMinutos = null;
	//public MedicoViewModel2025 MedicoRelacionado => App.BaseDeDatos.GetMedicoById(MedicoId ?? throw new Exception("El ID del médico es nulo."));
	//public WindowModificarPacienteDto PacienteRelacionado => App.BaseDeDatos.GetPacienteById(PacienteId ?? throw new Exception("El ID del paciente es nulo."));

	public static TurnoViewModel2025 NewEmpty() => new(
		id: default,
		pacienteId: default,
		medicoId: default,
		fecha: null,
		hora: string.Empty,
		duracionMinutos: null
	);

    internal ResultWpf<Turno2025> ToDomain() {
        throw new NotImplementedException();
    }

    public TurnoViewModel2025(
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
