using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Clinica.Dominio.TiposDeEnum;
using static Clinica.Shared.DbModels.DbModels;
namespace Clinica.AppWPF.UsuarioAdministrativo;



public sealed class GestionUsuariosVM : INotifyPropertyChanged {

	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	public event PropertyChangedEventHandler? PropertyChanged;
	public ObservableCollection<AccionesDeUsuarioEnum> RolesViewModelList { get; } = [];

	private UsuarioDbModel? _selectedUsuario;
	public UsuarioDbModel? SelectedUsuario {
		get => _selectedUsuario;
		set {
			if (_selectedUsuario != value) {
				_selectedUsuario = value;
				OnPropertyChanged(nameof(SelectedUsuario));
				OnPropertyChanged(nameof(HayUsuarioSeleccionado));
				OnSelectedUsuarioChanged();
			}
		}
	}

	private async void OnSelectedUsuarioChanged() {
		await CargarRolesDeUsuarioSeleccionado();
	}

	private string _filtroUsuariosTexto = string.Empty;
	public string FiltroUsuariosTexto {
		get => _filtroUsuariosTexto;
		set {
			if (_filtroUsuariosTexto != value) {
				_filtroUsuariosTexto = value;
				OnPropertyChanged(nameof(FiltroUsuariosTexto));
				FiltrarUsuarios(); // Aplica el filtro cada vez que cambia el texto
			}
		}
	}


	// ================================================================
	// COLECCIONES
	// ================================================================
	private List<UsuarioDbModel> _todosLosUsuarios = []; // Copia completa para filtrar
	public ObservableCollection<UsuarioDbModel> UsuariosList { get; } = [];




	internal async Task RefrescarUsuariosAsync() {
		List<UsuarioDbModel> usuarios = await App.Repositorio.SelectUsuarios();

		_todosLosUsuarios = usuarios;
		FiltrarUsuarios();
	}

	private void FiltrarUsuarios() {
		UsuariosList.Clear();

		IEnumerable<UsuarioDbModel> origen;

		if (string.IsNullOrWhiteSpace(FiltroUsuariosTexto)) {
			origen = _todosLosUsuarios;
		} else {
			var texto = FiltroUsuariosTexto.Trim();

			origen = _todosLosUsuarios.Where(m =>
				(m.Nombre?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
				(m.Apellido?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
				m.EnumRole
					.ToString()
					.Contains(texto, StringComparison.CurrentCultureIgnoreCase)
			);
		}

		foreach (UsuarioDbModel usuario in origen)
			UsuariosList.Add(usuario);
	}


	public bool HayUsuarioSeleccionado => SelectedUsuario is not null;

	private async Task CargarRolesDeUsuarioSeleccionado() {
		RolesViewModelList.Clear();

		if (SelectedUsuario is null) {
			// MessageBox.Show("por que es null el selectusuario?"); // porque se actualizo el listview de usuarios tras usarse un filtro!
			return;
		}
		IReadOnlyCollection<AccionesDeUsuarioEnum> permisosAcciones = await App.Repositorio.SelectPermisosAccionesWhereEnumRole(SelectedUsuario.EnumRole);

		foreach (AccionesDeUsuarioEnum accion in permisosAcciones)
			RolesViewModelList.Add(accion);
	}


	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

//public class EnumRoleViewModel(UsuarioRoleEnum horarioDbModel) {
//	public int UsuarioId { get; } = horarioDbModel.UsuarioId.Valor;
//	public DayOfWeek DiaSemana { get; } = horarioDbModel.DiaSemana;
//	public string DiaSemanaDescripcion => DiaSemana.ATexto();
//	public TimeSpan HoraDesde { get; } = horarioDbModel.HoraDesde;
//	public string HoraDesdeStr => HoraDesde.ToString(@"hh\:mm");
//	public TimeSpan HoraHasta { get; } = horarioDbModel.HoraHasta;
//	public string HoraHastaStr => HoraHasta.ToString(@"hh\:mm");
//	public DateTime VigenteDesde { get; } = horarioDbModel.VigenteDesde;
//	public DateTime? VigenteHasta { get; } = horarioDbModel.VigenteHasta ?? DateTime.MaxValue;
//}