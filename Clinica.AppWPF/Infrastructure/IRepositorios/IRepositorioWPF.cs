namespace Clinica.AppWPF.Infrastructure.IRepositorios;



public interface IRepositorioWPF {
	IRepositorioMedicosWPF Medicos { get; }
	IRepositorioPacientesWPF Pacientes { get; }
	IRepositorioDominioWPF Dominio { get; }
	IRepositorioTurnosWPF Turnos { get; }
	IRepositorioHorariosWPF Horarios { get; }
	IRepositorioUsuariosWPF Usuarios { get; }
	IRepositorioAtencionesWPF Atenciones { get; }
}


