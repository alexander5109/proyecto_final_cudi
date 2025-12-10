using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.TiposDeValor;
using Clinica.Dominio.TiposExtensiones;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure;

public static class Comodidades {

	

	public record DisponibilidadEspecialidadModelView(string Fecha, string Hora, string MedicoDisplayear, DiaDeSemanaViewModel DiaSemana);
	public record EspecialidadViewModel(EspecialidadCodigo Codigo, string Displayear);
	public record MedicoSimpleViewModel(MedicoId Id, EspecialidadCodigo EspecialidadCodigo, string Displayear);
	//public record ModelViewDiaSemana(int Value, string NombreDia);


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
		MedicoDbModel? medico = RepoCache.DictMedicos.GetValueOrDefault(domainValue.MedicoId);
		return new DisponibilidadEspecialidadModelView(
			Fecha: domainValue.FechaHoraDesde.ATextoHoras(),
			Hora: domainValue.FechaHoraDesde.ATextoHoras(),
			MedicoDisplayear: $"{medico?.Nombre}{medico?.Apellido}",
			DiaSemana: new DiaDeSemanaViewModel(domainValue.FechaHoraDesde.DayOfWeek, domainValue.FechaHoraDesde.DayOfWeek.ATexto())
		);
	}


}
