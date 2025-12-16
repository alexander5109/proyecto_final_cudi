using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.TiposDeEnum;
using static Clinica.Shared.DbModels.DbModels;
namespace Clinica.AppWPF.UsuarioAdministrativo;



public sealed class GestionUsuariosVM : INotifyPropertyChanged {

	// ==========================================================
	// BOTONES: NAV
	// ==========================================================
	public event PropertyChangedEventHandler? PropertyChanged;
	public ObservableCollection<PermisosViewModel> RolesViewModelList { get; } = [];

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
		IReadOnlyCollection<AccionesDeUsuarioEnum> permisosAcciones = await App.Repositorio.SelectAccionesDeUsuario();
		//IReadOnlyCollection<AccionesDeUsuarioEnum> permisosAcciones = await App.Repositorio.SelectAccionesDeUsuarioWhereEnumRole(SelectedUsuario.EnumRole);

		foreach (AccionesDeUsuarioEnum accionEnum in permisosAcciones)
			RolesViewModelList.Add(new PermisosViewModel(accionEnum, SelectedUsuario.EnumRole.TienePermisosPara(accionEnum)));
			//RolesViewModelList.Add(new PermisosViewModel(SelectedUsuario.EnumRole, accionEnum, SelectedUsuario.EnumRole.TienePermisosPara(accionEnum)));
	}


	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class PermisosViewModel(AccionesDeUsuarioEnum accionEnum, bool tienePermiso) {
//public class PermisosViewModel(UsuarioRoleEnum usuarioRoleEnum, AccionesDeUsuarioEnum accionEnum, bool tienePermiso) {
	//public string Rol { get; } = usuarioRoleEnum.ToString();
	public string Accion { get; } = accionEnum.ToString();
	public string TienePermiso => tienePermiso is false ? "No" : "Si";
}