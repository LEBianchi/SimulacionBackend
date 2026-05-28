namespace SimulacionBackend.DTOs
{
    public class SimulacionResultDTO
    {
        public int TotalEquiposIngresados { get; set; }
        public int EquiposReacondicionados { get; set; }
        public int EquiposDesmantelados { get; set; }
        public double KilosPlasticoRecuperado { get; set; }
        public double KilosMetalRecuperado { get; set; }
        public double TiempoPromedioEspera { get; set; }
    }
}
