using SimulacionBackend.DTOs;
using SimulacionBackend.Generadores;
using SimulacionBackend.Data;
using System;

namespace SimulacionBackend.Services
{
    public class SimulacionService : ISimulacionService
    {
        private readonly AppDbContext _context;

        public SimulacionService(AppDbContext context)
        {
            _context = context;
        }

        public SimulacionResultDTO EjecutarSimulacion(SimulacionRequestDTO parametros)
        {
            long semilla = DateTime.Now.Ticks % int.MaxValue;
            var congruencial = new CongruencialMixto(semilla, 1664525, 1013904223, (long)Math.Pow(2, 32));
            var dist = new Distribuciones(congruencial);

            int contador_tablets = 0, contador_celulares = 0;
            int cant_min = 0, cant_rec = 0;
            double TUT = 0, temp_min = 0, temp_borrado = 0, temp_arreglo = 0;
            double total_metales = 0, total_plastico = 0;

            // Variables de colas (Backlog)
            int cola_triage = 0, cola_mineria = 0, cola_reacond = 0;

            // Capacidad máxima en MINUTOS por jornada
            double maxMinutosTriage = parametros.HorasJornada * 60.0 * parametros.EmpleadosTriage;
            double maxMinutosMineria = parametros.HorasJornada * 60.0 * parametros.EmpleadosDesmantelamiento;
            double maxMinutosReacond = parametros.HorasJornada * 60.0 * parametros.EmpleadosReaciclaje;

            // Generamos los lotes que llegan en TODO EL DÍA
            int lotesDelDia = dist.Poisson(4);

            for (int j = 1; j <= lotesDelDia; j++)
            {
                double kgLote = dist.Normal(120, 15);
                double kg_telTabl = kgLote * 0.08;
                double peso_acumulado = 0;

                while (peso_acumulado < kg_telTabl)
                {
                    double u_tipo = congruencial.obtenerSiguiente();

                    // Pesos realistas: Celulares (Media 200g, Desv 20g) | Tablets (Media 600g, Desv 50g)
                    double peso_actual = u_tipo <= 0.7 ? dist.Normal(0.200, 0.02) : dist.Normal(0.600, 0.05);

                    // Freno de seguridad por si la estadística escupe un negativo rarísimo
                    if (peso_actual <= 0) peso_actual = 0.05;

                    peso_acumulado += peso_actual;

                    // --- 1. INTENTAR TRIAGE ---
                    double t_triage = dist.Uniforme(15, 20);
                    if (TUT + t_triage > maxMinutosTriage)
                    {
                        cola_triage++;
                        continue; // El empleado no da abasto, el equipo queda en la caja
                    }

                    TUT += t_triage;
                    if (u_tipo <= 0.7) contador_celulares++; else contador_tablets++;

                    // --- 2. RUTEO A MINERÍA O REACONDICIONAMIENTO ---
                    double u_destino = congruencial.obtenerSiguiente();
                    if (u_destino <= 0.1)
                    {
                        /* Eliminación Directa */
                    }
                    else if (u_destino <= 0.25)
                    {
                        bool exito = IntentarReacondicionamiento(congruencial, dist, maxMinutosReacond, ref cant_rec, ref temp_borrado, ref temp_arreglo);
                        if (!exito) cola_reacond++;
                    }
                    else
                    {
                        bool exito = IntentarMineria(dist, peso_actual, maxMinutosMineria, ref cant_min, ref temp_min, ref total_metales, ref total_plastico);
                        if (!exito) cola_mineria++;
                    }
                }
            }

            // Cálculos finales
            double tiempoTotalHoras = (TUT + temp_min + temp_borrado + temp_arreglo) / 60.0;

            double eficTriage = maxMinutosTriage > 0 ? (TUT / maxMinutosTriage) * 100 : 0;
            double eficMineria = maxMinutosMineria > 0 ? (temp_min / maxMinutosMineria) * 100 : 0;
            double eficReacond = maxMinutosReacond > 0 ? ((temp_borrado + temp_arreglo) / maxMinutosReacond) * 100 : 0;

            // Armamos el registro para la Base de Datos
            var registro = new SimulacionRecord
            {
                // Ingresados reales (Los que procesó + los que quedaron juntando polvo)
                TotalEquiposIngresados = contador_celulares + contador_tablets + cola_triage,
                EquiposReacondicionados = cant_rec,
                EquiposDesmantelados = cant_min,
                KilosMetalRecuperado = Math.Round(total_metales, 2),
                KilosPlasticoRecuperado = Math.Round(total_plastico, 2),
                TiempoPromedioEspera = Math.Round(tiempoTotalHoras, 2),
                EficienciaTriage = Math.Round(eficTriage, 2),
                EficienciaDesmantelamiento = Math.Round(eficMineria, 2),
                EficienciaReacondicionamiento = Math.Round(eficReacond, 2),
                EquiposEnColaTriage = cola_triage,
                EquiposEnColaDesmantelamiento = cola_mineria,
                EquiposEnColaReacondicionamiento = cola_reacond
            };

            // Guardamos en SQLite
            _context.Simulaciones.Add(registro);
            _context.SaveChanges();

            // Devolvemos el JSON al Frontend
            return new SimulacionResultDTO
            {
                TotalEquiposIngresados = registro.TotalEquiposIngresados,
                EquiposReacondicionados = registro.EquiposReacondicionados,
                EquiposDesmantelados = registro.EquiposDesmantelados,
                KilosMetalRecuperado = registro.KilosMetalRecuperado,
                KilosPlasticoRecuperado = registro.KilosPlasticoRecuperado,
                TiempoPromedioEspera = registro.TiempoPromedioEspera,
                EficienciaTriage = registro.EficienciaTriage,
                EficienciaDesmantelamiento = registro.EficienciaDesmantelamiento,
                EficienciaReacondicionamiento = registro.EficienciaReacondicionamiento,
                EquiposEnColaTriage = registro.EquiposEnColaTriage,
                EquiposEnColaDesmantelamiento = registro.EquiposEnColaDesmantelamiento,
                EquiposEnColaReacondicionamiento = registro.EquiposEnColaReacondicionamiento
            };
        }

        // =========================================================================
        // MÉTODOS AUXILIARES
        // =========================================================================

        private bool IntentarMineria(Distribuciones dist, double peso_actual, double maxMinutos, ref int cant_min, ref double temp_min, ref double total_metales, ref double total_plastico)
        {
            double t_min = dist.Uniforme(25, 40);

            // Si el tiempo del área ya no da más, no procesamos este equipo
            if (temp_min + t_min > maxMinutos) return false;

            cant_min++;
            temp_min += t_min;
            total_metales += (peso_actual * 0.35);
            total_plastico += (peso_actual * 0.40);
            return true;
        }

        private bool IntentarReacondicionamiento(CongruencialMixto congruencial, Distribuciones dist, double maxMinutos, ref int cant_rec, ref double temp_borrado, ref double temp_arreglo)
        {
            double t_borrado = dist.Uniforme(30, 45);
            double t_arreglo = congruencial.obtenerSiguiente() <= 0.65 ? dist.Exponencial(90) : dist.Exponencial(180);

            // Verificamos si hay tiempo físico para hacer todo el proceso
            if (temp_borrado + temp_arreglo + t_borrado + t_arreglo > maxMinutos) return false;

            cant_rec++;
            temp_borrado += t_borrado;
            temp_arreglo += t_arreglo;
            return true;
        }
    }
}