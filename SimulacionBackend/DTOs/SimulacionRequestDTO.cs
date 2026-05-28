namespace SimulacionBackend.DTOs
{
    public class SimulacionRequestDTO
    {
        public int HorasJornada { get; set; } = 8;
        public int EmpleadosTriage { get; set; } = 1;
        public int EmpleadosDesmantelamiento { get; set; } = 1;
        public int EmpleadosReaciclaje { get; set; } = 1;
    }
}
