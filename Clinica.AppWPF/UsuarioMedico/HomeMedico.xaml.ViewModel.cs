using System.ComponentModel;
using System.Threading.Tasks;
using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.DbModels;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioMedico {
	public class HomeMedicoVM : INotifyPropertyChanged {
		public event PropertyChangedEventHandler? PropertyChanged;

		private string _mensajeBienvenida = "";
		public string MensajeBienvenida {
			get => _mensajeBienvenida;
			set {
				if (_mensajeBienvenida != value) {
					_mensajeBienvenida = value;
					OnPropertyChanged(nameof(MensajeBienvenida));
				}
			}
		}

		private bool _isSoundOn;
		public bool IsSoundOn {
			get => _isSoundOn;
			set {
				if (_isSoundOn != value) {
					_isSoundOn = value;
					SoundsService.SoundOn = value; // actualiza el servicio
					OnPropertyChanged(nameof(IsSoundOn));
				}
			}
		}

		public HomeMedicoVM() {
			// inicializar con estado actual del servicio
			_isSoundOn = SoundsService.SoundOn;
		}

		public async Task CargarMiPerfil(MedicoId2025 medicoId) {
			if (App.Repositorio?.Medicos == null) {
				MensajeBienvenida = "Error: repositorio no inicializado.";
				return;
			}

			MedicoDbModel? medico = await App.Repositorio.Medicos.SelectMedicoWhereId(medicoId);
			if (medico != null) {
				MensajeBienvenida = $"Bienvenid@ {medico.Nombre} {medico.Apellido}\nEspecialidad: {medico.EspecialidadCodigo}";
			} else {
				MensajeBienvenida = "No se encontró información del médico.";
			}
		}

		private void OnPropertyChanged(string propName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
	}
}
