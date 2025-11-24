using System;
using System.Collections.Generic;
using System.Text;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.Dtos;

namespace Clinica.Infrastructure.ServiciosAsync;

public partial class ServiciosPublicosAsync {

	private Func<EspecialidadMedica2025, IEnumerable<Medico2025>> FunctorSelectMedicosWhereEspecialidad() {
		return especialidad => {
			return Enumerar();
			IEnumerable<Medico2025> Enumerar() {
				IEnumerable<MedicoDto> dtos = BaseDeDatos.SelectMedicosWhereEspecialidad(especialidad).Result;
				foreach (MedicoDto dto in dtos) {
					Result<Medico2025> dom = dto.ToDomain();
					if (dom is Result<Medico2025>.Ok ok)
						yield return ok.Valor;
				}
			}
		};
	}


	private Func<MedicoId, DateTime, DateTime, IEnumerable<Turno2025>> FunctorSelectTurnosWhereMedicoIdBetweenFechas() {
		return (medicoId, fechaDesde, fechaHasta) => {
			return Enumerar();

			IEnumerable<Turno2025> Enumerar() {
				IEnumerable<TurnoDto> dtos = BaseDeDatos
					.SelectTurnosWhereMedicoIdBetweenFechas(medicoId, fechaDesde, fechaHasta)
					.Result;

				foreach (TurnoDto dto in dtos) {
					Result<Turno2025> dom = dto.ToDomain();
					if (dom is Result<Turno2025>.Ok ok)
						yield return ok.Valor;
				}
			}
		};
	}

	private Func<MedicoId, DateTime, DateTime, IEnumerable<HorarioMedico2025>>
		FunctorSelectHorariosWhereMedicoIdInVigencia() {
		return (medicoId, fechaDesde, fechaHasta) => {
			return Enumerar();

			IEnumerable<HorarioMedico2025> Enumerar() {
				var dtos = BaseDeDatos
					.SelectHorariosWhereMedicoIdInVigencia(medicoId, fechaDesde, fechaHasta)
					.Result;

				foreach (var dto in dtos) {
					var dom = dto.ToDomain();
					if (dom is Result<HorarioMedico2025>.Ok ok)
						yield return ok.Valor;
				}
			}
		};
	}





}
