using Clinica.AppWPF.ViewModels;
using System.Configuration;
using System.IO;
using System.Windows;


namespace Clinica.AppWPF.Infrastructure;

public class BaseDeDatosWebAPI : BaseDeDatosInterface {
	bool BaseDeDatosInterface.CreateMedico(WindowModificarMedicoViewModel instance) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.CreatePaciente(WindowModificarPacienteViewModel instance) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.CreateTurno(WindowModificarTurnoViewModel instance) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.DeleteMedico(WindowModificarMedicoViewModel instance) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.DeletePaciente(WindowModificarPacienteViewModel instance) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.DeleteTurno(WindowModificarTurnoViewModel instance) {
		throw new NotImplementedException();
	}

	EspecialidadMedicaViewModel BaseDeDatosInterface.GetEspecialidadById(int id) {
		throw new NotImplementedException();
	}

	WindowModificarMedicoViewModel BaseDeDatosInterface.GetMedicoById(int id) {
		throw new NotImplementedException();
	}

	WindowModificarPacienteViewModel BaseDeDatosInterface.GetPacienteById(int id) {
		throw new NotImplementedException();
	}

	WindowModificarTurnoViewModel BaseDeDatosInterface.GetTurnoById(int id) {
		throw new NotImplementedException();
	}

	List<EspecialidadMedicaViewModel> BaseDeDatosInterface.ReadDistinctEspecialidades() {
		throw new NotImplementedException();
	}

	List<WindowModificarMedicoViewModel> BaseDeDatosInterface.ReadMedicos() {
		throw new NotImplementedException();
	}

	List<WindowModificarMedicoViewModel> BaseDeDatosInterface.ReadMedicosWhereEspecialidad(int? especialidadCodigoInterno) {
		throw new NotImplementedException();
	}

	List<WindowModificarPacienteViewModel> BaseDeDatosInterface.ReadPacientes() {
		throw new NotImplementedException();
	}

	List<WindowModificarTurnoViewModel> BaseDeDatosInterface.ReadTurnos() {
		throw new NotImplementedException();
	}

	List<WindowModificarTurnoViewModel> BaseDeDatosInterface.ReadTurnosWhereMedicoId(int? medicoId) {
		throw new NotImplementedException();
	}

	List<WindowModificarTurnoViewModel> BaseDeDatosInterface.ReadTurnosWherePacienteId(int? pacienteId) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.UpdateMedico(WindowModificarMedicoViewModel instance) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.UpdatePaciente(WindowModificarPacienteViewModel instance) {
		throw new NotImplementedException();
	}

	bool BaseDeDatosInterface.UpdateTurno(WindowModificarTurnoViewModel instance) {
		throw new NotImplementedException();
	}
}
