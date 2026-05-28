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

        public double EficienciaTriage { get; set; }
        public double EficienciaDesmantelamiento { get; set; }
        public double EficienciaReacondicionamiento { get; set; }
        public int EquiposEnColaTriage { get; set; }
        public int EquiposEnColaDesmantelamiento { get; set; }
        public int EquiposEnColaReacondicionamiento { get; set; }
    }
}