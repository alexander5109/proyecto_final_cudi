using System.Windows;
using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.AppWPF.UsuarioSecretaria;

public static class Comodidades {

	

	public record DisponibilidadEspecialidadModelView(string Fecha, string Hora, string Medico, DiaDeSemanaViewModel DiaSemana);
	public record EspecialidadViewModel(EspecialidadCodigo Codigo, string Displayear);
	public record MedicoSimpleViewModel(MedicoId Id, EspecialidadCodigo EspecialidadCodigo, string Displayear);
	//public record ModelViewDiaSemana(int Value, string NombreDia);


	public static async Task<MedicoDbModel> RespectivoMedico(this MedicoId id) {
        MedicoDbModel? instance = await App.Repositorio.SelectMedicoWhereId(id);
		if (instance is not null) return instance;
		string error = $"No existe el médico con ID {id.Valor}";
		MessageBox.Show(error);
		throw new InvalidOperationException(error);
	}
	public static async Task<PacienteDbModel> RespectivoPaciente(this PacienteId id) {
		PacienteDbModel? instance = await App.Repositorio.SelectPacienteWhereId(id);
		if (instance is not null) return instance;
		string error = $"No existe el médico con ID {id.Valor}";
		MessageBox.Show(error);
		throw new InvalidOperationException(error);
	}

	public static MedicoSimpleViewModel ToSimpleViewModel(this MedicoDbModel model) {
		return new MedicoSimpleViewModel(
			Id: model.Id,
			EspecialidadCodigo: model.EspecialidadCodigo,
			Displayear: $"{model.Nombre} {model.Apellido}"
		);
	}

	public static EspecialidadViewModel ToSimpleViewModel(this Especialidad2025 instance) {
		return new EspecialidadViewModel(
			Codigo: instance.Codigo,
			Displayear: $"{instance.Titulo} --- (Duración consulta: {instance.DuracionConsultaMinutos})"
		);
	}

	async public static Task<DisponibilidadEspecialidadModelView> ToSimpleViewModel(this Disponibilidad2025 domainValue) {
		MedicoDbModel medico = await domainValue.MedicoId.RespectivoMedico();
		return new DisponibilidadEspecialidadModelView(
			Fecha: domainValue.FechaHoraDesde.ATextoHoras(),
			Hora: domainValue.FechaHoraDesde.ATextoHoras(),
			Medico: $"{medico.Nombre}{medico.Apellido}",
			DiaSemana: new DiaDeSemanaViewModel(domainValue.FechaHoraDesde.DayOfWeek, domainValue.FechaHoraDesde.DayOfWeek.ATexto())
		);
	}


}
