using System.ComponentModel;
using Clinica.AppWPF;
using Clinica.AppWPF.Infrastructure;
using Clinica.AppWPF.UsuarioRecepcionista;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.AppWPF.UsuarioRecepcionista.RecepcionistaPacienteMiniViewModels;


public class RecepcionistaPacienteFormularioViewModel : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;
	private void Notify(string prop) => PropertyChanged?.Invoke(this, new(prop));

	// -----------------------------
	// PROPIEDADES
	// -----------------------------

	public bool EstaCreando => Id is null;
	public bool EstaEditando => Id is not null;
	public bool PuedeGuardarCambios => true;
	//public bool PuedeGuardarCambios => true;


	private PacienteId? _id;
	public PacienteId? Id {
		get => _id;
		set { _id = value; 
			Notify(nameof(Id));
			Notify(nameof(EstaCreando));
			Notify(nameof(EstaEditando));
		}
	}

	private string _dni = "";
	public string Dni {
		get => _dni;
		set { _dni = value; Notify(nameof(Dni)); }
	}

	private string _nombre = "";
	public string Nombre {
		get => _nombre;
		set { _nombre = value; Notify(nameof(Nombre)); }
	}

	private string _apellido = "";
	public string Apellido {
		get => _apellido;
		set { _apellido = value; Notify(nameof(Apellido)); }
	}

	private DateTime? _fechaIngreso;
	public DateTime? FechaIngreso {
		get => _fechaIngreso;
		set { _fechaIngreso = value; Notify(nameof(FechaIngreso)); }
	}

	private string _domicilio = "";
	public string Domicilio {
		get => _domicilio;
		set { _domicilio = value; Notify(nameof(Domicilio)); }
	}

	private string _localidad = "";
	public string Localidad {
		get => _localidad;
		set { _localidad = value; Notify(nameof(Localidad)); }
	}

	private ProvinciaVmItem? _provincia;
	public ProvinciaVmItem? Provincia {
		get => _provincia;
		set { _provincia = value; Notify(nameof(Provincia)); Notify(nameof(Provincias)); }
	}

	public IReadOnlyList<ProvinciaVmItem> Provincias { get; } = [.. ProvinciaArgentina2025.Todas().Select(p => p.ToViewModel())];


	private string _telefono = "";
	public string Telefono {
		get => _telefono;
		set { _telefono = value; Notify(nameof(Telefono)); }
	}

	private string _email = "";
	public string Email {
		get => _email;
		set { _email = value; Notify(nameof(Email)); }
	}

	private DateTime _fechaNacimiento = DateTime.Today;
	public DateTime FechaNacimiento {
		get => _fechaNacimiento;
		set { _fechaNacimiento = value; Notify(nameof(FechaNacimiento)); }
	}

	public string Displayear => $"{Id}: {Nombre} {Apellido}";



	public async Task<ResultWpf<UnitWpf>> GuardarAsync() {
		return await this.ToDomain(fechaIngreso:DateTime.Now).Bind(async paciente =>
		{
			if (Id is PacienteId idExistente) {
				Paciente2025Agg agg = new(idExistente, paciente);
				return await App.Repositorio.UpdatePacienteWhereId(agg);
			} else {
				//MessageBox.Show(paciente.FechaIngreso);
				//MessageBox.Show(paciente.FechaNacimiento);

				return (await App.Repositorio.InsertPacienteReturnId(paciente))
					.MatchTo<PacienteId, UnitWpf>(
						ok => {
							Id = ok;
							return new ResultWpf<UnitWpf>.Ok(UnitWpf.Valor);
						},
						error => new ResultWpf<UnitWpf>.Error(error)
					);
			}
		});
	}





}
