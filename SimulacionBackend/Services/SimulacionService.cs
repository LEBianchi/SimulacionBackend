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

            // Contadores Generales
            int contador_tablets = 0, contador_celulares = 0;
            int cant_min = 0, cant_rec = 0;

            // Contadores Específicos Nuevos
            int cel_desmantelados = 0, tab_desmanteladas = 0;
            int cel_reacond = 0, tab_reacond = 0;

            double TUT = 0, temp_min = 0, temp_borrado = 0, temp_arreglo = 0;
            double total_metales = 0, total_plastico = 0;
            double total_basura_cruda = 0; // NUEVA VARIABLE: Kilos brutos que llegan en el camión

            // Variables de colas
            int cola_triage = 0, cola_mineria = 0, cola_reacond = 0;

            double maxMinutosTriage = parametros.HorasJornada * 60.0 * parametros.EmpleadosTriage;
            double maxMinutosMineria = parametros.HorasJornada * 60.0 * parametros.EmpleadosDesmantelamiento;
            double maxMinutosReacond = parametros.HorasJornada * 60.0 * parametros.EmpleadosReaciclaje;

            int lotesDelDia = dist.Poisson(4);

            for (int j = 1; j <= lotesDelDia; j++)
            {
                double kgLote = dist.Normal(120, 15);

                // ACÁ SUMAMOS EL PESO DEL CAMIÓN A LA BASURA CRUDA
                total_basura_cruda += kgLote;

                double kg_telTabl = kgLote * 0.08;
                double peso_acumulado = 0;

                while (peso_acumulado < kg_telTabl)
                {
                    double u_tipo = congruencial.obtenerSiguiente();
                    bool isCelular = u_tipo <= 0.7; // Variable para saber qué es

                    // Contamos los que ingresan a la planta NI BIEN ENTRAN
                    if (isCelular) contador_celulares++;
                    else contador_tablets++;

                    double peso_actual = isCelular ? dist.Normal(0.200, 0.02) : dist.Normal(0.600, 0.05);
                    if (peso_actual <= 0) peso_actual = 0.05;

                    peso_acumulado += peso_actual;

                    double t_triage = dist.Uniforme(15, 20);
                    if (TUT + t_triage > maxMinutosTriage)
                    {
                        cola_triage++;
                        continue;
                    }

                    TUT += t_triage;

                    double u_destino = congruencial.obtenerSiguiente();
                    if (u_destino <= 0.1)
                    {
                        // Basura directa, no suma a minería ni reacondicionamiento
                    }
                    else if (u_destino <= 0.25)
                    {
                        bool exito = IntentarReacondicionamiento(congruencial, dist, maxMinutosReacond, ref cant_rec, ref temp_borrado, ref temp_arreglo);
                        if (!exito)
                        {
                            cola_reacond++;
                        }
                        else
                        {
                            // Si tuvo éxito, sumamos al contador específico
                            if (isCelular) cel_reacond++; else tab_reacond++;
                        }
                    }
                    else
                    {
                        bool exito = IntentarMineria(dist, peso_actual, maxMinutosMineria, ref cant_min, ref temp_min, ref total_metales, ref total_plastico);
                        if (!exito)
                        {
                            cola_mineria++;
                        }
                        else
                        {
                            // Si tuvo éxito, sumamos al contador específico
                            if (isCelular) cel_desmantelados++; else tab_desmanteladas++;
                        }
                    }
                }
            }

            double tiempoTotalHoras = (TUT + temp_min + temp_borrado + temp_arreglo) / 60.0;
            double eficTriage = maxMinutosTriage > 0 ? (TUT / maxMinutosTriage) * 100 : 0;
            double eficMineria = maxMinutosMineria > 0 ? (temp_min / maxMinutosMineria) * 100 : 0;
            double eficReacond = maxMinutosReacond > 0 ? ((temp_borrado + temp_arreglo) / maxMinutosReacond) * 100 : 0;

            var registro = new SimulacionRecord
            {
                TotalEquiposIngresados = contador_celulares + contador_tablets,
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
                EquiposEnColaReacondicionamiento = cola_reacond,

                // Mapeo del nuevo campo de basura cruda
                KilosBasuraFisicaTotal = Math.Round(total_basura_cruda, 2),

                // Mapeo de los campos a la BD
                CelularesIngresados = contador_celulares,
                TabletsIngresadas = contador_tablets,
                CelularesDesmantelados = cel_desmantelados,
                TabletsDesmanteladas = tab_desmanteladas,
                CelularesReacondicionados = cel_reacond,
                TabletsReacondicionadas = tab_reacond
            };

            _context.Simulaciones.Add(registro);
            _context.SaveChanges();

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
                EquiposEnColaReacondicionamiento = registro.EquiposEnColaReacondicionamiento,

                // Mapeo del nuevo campo a React
                KilosBasuraFisicaTotal = registro.KilosBasuraFisicaTotal,

                // Mapeo de los campos a React
                CelularesIngresados = registro.CelularesIngresados,
                TabletsIngresadas = registro.TabletsIngresadas,
                CelularesDesmantelados = registro.CelularesDesmantelados,
                TabletsDesmanteladas = registro.TabletsDesmanteladas,
                CelularesReacondicionados = registro.CelularesReacondicionados,
                TabletsReacondicionadas = registro.TabletsReacondicionadas
            };
        }

        private bool IntentarMineria(Distribuciones dist, double peso_actual, double maxMinutos, ref int cant_min, ref double temp_min, ref double total_metales, ref double total_plastico)
        {
            double t_min = dist.Uniforme(25, 40);
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

            if (temp_borrado + temp_arreglo + t_borrado + t_arreglo > maxMinutos) return false;

            cant_rec++;
            temp_borrado += t_borrado;
            temp_arreglo += t_arreglo;
            return true;
        }
    }
}