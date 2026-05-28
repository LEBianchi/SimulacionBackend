using SimulacionBackend.DTOs;

namespace SimulacionBackend.Services
{
    public interface ISimulacionService
    {
        SimulacionResultDTO EjecutarSimulacion(SimulacionRequestDTO parametros);
    }
}