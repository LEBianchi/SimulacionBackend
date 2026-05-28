using Microsoft.AspNetCore.Mvc;
using SimulacionBackend.Data;
using SimulacionBackend.DTOs;
using SimulacionBackend.Services;

namespace SimulacionBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulacionController : ControllerBase
    {
        private readonly ISimulacionService _simulacionService;
        private readonly AppDbContext _context;

        // El constructor recibe el servicio que inyectamos en el Program.cs
        public SimulacionController(ISimulacionService simulacionService, AppDbContext context)
        {
            _simulacionService = simulacionService;
            _context = context;
        }

        [HttpPost("ejecutar")]
        public ActionResult<SimulacionResultDTO> Ejecutar([FromBody] SimulacionRequestDTO parametros)
        {
            try
            {
                var resultado = _simulacionService.EjecutarSimulacion(parametros);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al correr la simulación: {ex.Message}");
            }
        }
        [HttpGet("historial")]
        public IActionResult ObtenerHistorial()
        {
            try
            {
                
                var historial = _context.Simulaciones
                    .OrderByDescending(s => s.FechaEjecucion)
                    .ToList();

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el historial: {ex.Message}");
            }
        }
    }
}