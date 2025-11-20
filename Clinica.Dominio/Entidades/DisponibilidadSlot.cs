using System;

namespace Clinica.Dominio.Entidades;

public record DisponibilidadSlot(DateTime FechaHora, int? MedicoId, string MedicoDisplay);
