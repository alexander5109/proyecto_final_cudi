using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using System.Collections.Generic;

namespace Clinica.Dominio.Repositorios;

public interface IRepositorioMedicos
{
    // Devuelve todos los médicos que ejercen la especialidad indicada (por título)
    Result<IReadOnlyList<Medico2025>> ObtenerMedicosPorEspecialidad(string especialidadTitulo);

    // Buscar por DNI
    Result<Medico2025> ObtenerPorDni(string dni);

    // Guardar/actualizar (implementación externa)
    Result<Medico2025> Guardar(Medico2025 medico);

    Result<bool> EliminarPorDni(string dni);
}
