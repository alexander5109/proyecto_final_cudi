using Clinica.AppWPF.Infrastructure.IRepositorios;

namespace Clinica.AppWPF.Infrastructure.Repositorio;

public sealed class RepositorioWPF : IRepositorioWPF {

	public IRepositorioMedicosWPF Medicos { get; }
	public IRepositorioPacientesWPF Pacientes { get; }
	public IRepositorioDominioWPF Dominio { get; }
	public IRepositorioTurnosWPF Turnos { get; }
	public IRepositorioHorariosWPF Horarios { get; }
	public IRepositorioUsuariosWPF Usuarios { get; }

	public RepositorioWPF() {
		Medicos = new RepositorioMedicosWPF();
		Pacientes = new RepositorioPacientesWPF();
		Dominio = new RepositorioDominioWPF();
		Turnos = new RepositorioTurnosWPF();
		Horarios = new RepositorioHorariosWPF();
		Usuarios = new RepositorioUsuariosWPF();
	}
}
