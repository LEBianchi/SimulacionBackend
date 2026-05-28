using System;
using System.ComponentModel.DataAnnotations;

namespace SimulacionBackend.Data
{
    public class SimulacionRecord
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaEjecucion { get; set; } = DateTime.Now;
        public int TotalEquiposIngresados { get; set; }
        public int EquiposReacondicionados { get; set; }
        public int EquiposDesmantelados { get; set; }
        public double KilosPlasticoRecuperado { get; set; }
        public double KilosMetalRecuperado { get; set; }
        public double TiempoPromedioEspera { get; set; }
    }
}