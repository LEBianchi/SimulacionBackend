using SimulacionBackend.DTOs;
using SimulacionBackend.Generadores;
using SimulacionBackend.Data;
using System;
using System.Linq;

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
            // Generador pseudoaleatorio basado en el método de Lehmer
            long semilla = DateTime.Now.Ticks % int.MaxValue;
            var congruencial = new CongruencialMixto(semilla, 1664525, 1013904223, (long)Math.Pow(2, 32));
            var dist = new Distribuciones(congruencial);

            // Contadores Generales
            int contador_tablets = 0, contador_celulares = 0;
            int cant_min = 0, cant_rec = 0;

            // Contadores Específicos
            int cel_desmantelados = 0, tab_desmanteladas = 0;
            int cel_reacond = 0, tab_reacond = 0;

            double TUT = 0, temp_min = 0, temp_borrado = 0, temp_arreglo = 0;
            double total_metales = 0, total_plastico = 0;
            double total_basura_cruda = 0;

            // Variables de colas y tiempos
            int cola_triage = 0, cola_mineria = 0, cola_reacond = 0;
            double tiempo_espera_total_minutos = 0; // Acumulador de espera real (Wq)

            // CORRECCIÓN: Relojes individuales por operario para simular canales en paralelo (M/M/c)
            double[] relojes_triage = new double[parametros.EmpleadosTriage];
            double[] relojes_mineria = new double[parametros.EmpleadosDesmantelamiento];
            double[] relojes_reacond = new double[parametros.EmpleadosReaciclaje];

            // El límite de tiempo real de la jornada (ej: 8 horas = 480 minutos)
            double finJornadaMinutos = parametros.HorasJornada * 60.0;

            int lotesDelDia = dist.Poisson(4);
            double tiempo_llegada_acumulado = 0;

            for (int j = 1; j <= lotesDelDia; j++)
            {
                // Simulamos la llegada de los camiones durante la jornada
                tiempo_llegada_acumulado += dist.Uniforme(30, 120);

                double kgLote = dist.Normal(120, 15);
                total_basura_cruda += kgLote;

                double kg_telTabl = kgLote * 0.08;
                double peso_acumulado = 0;

                while (peso_acumulado < kg_telTabl)
                {
                    double u_tipo = congruencial.obtenerSiguiente();
                    bool isCelular = u_tipo <= 0.7;

                    if (isCelular) contador_celulares++;
                    else contador_tablets++;

                    double peso_actual = isCelular ? dist.Normal(0.200, 0.02) : dist.Normal(0.600, 0.05);
                    if (peso_actual <= 0) peso_actual = 0.05;

                    peso_acumulado += peso_actual;

                    // 1. TRIAGE
                    double t_triage = dist.Uniforme(15, 20);

                    // CORRECCIÓN PARALELO: Buscamos al operario de Triage que se libere primero
                    int indiceOperarioTriage = ObtenerOperarioMasLibre(relojes_triage);
                    double reloj_operario_triage = relojes_triage[indiceOperarioTriage];

                    double inicio_triage = Math.Max(tiempo_llegada_acumulado, reloj_operario_triage);
                    double espera_triage = inicio_triage - tiempo_llegada_acumulado;

                    // Si el operario libre termina después del fin de jornada, se va a cola
                    if (inicio_triage + t_triage > finJornadaMinutos)
                    {
                        cola_triage++;
                        double espera_restante = finJornadaMinutos - tiempo_llegada_acumulado;
                        if (espera_restante > 0)
                        {
                            tiempo_espera_total_minutos += espera_restante;
                        }
                        continue;
                    }

                    tiempo_espera_total_minutos += espera_triage;
                    relojes_triage[indiceOperarioTriage] = inicio_triage + t_triage;
                    TUT += t_triage;

                    // 2. DESTINO
                    double u_destino = congruencial.obtenerSiguiente();
                    double tiempo_salida_triage = relojes_triage[indiceOperarioTriage];

                    if (u_destino <= 0.1)
                    {
                        // Basura directa (rechazo)
                    }
                    else if (u_destino <= 0.25)
                    {
                        // REACONDICIONAMIENTO
                        double t_borrado = dist.Uniforme(30, 45);
                        double t_arreglo = congruencial.obtenerSiguiente() <= 0.65 ? dist.Exponencial(90) : dist.Exponencial(180);
                        double t_total_reacond = t_borrado + t_arreglo;

                        // CORRECCIÓN PARALELO: Buscamos operario libre en Reacondicionamiento
                        int indiceOpReacond = ObtenerOperarioMasLibre(relojes_reacond);
                        double reloj_operario_reacond = relojes_reacond[indiceOpReacond];

                        double inicio_reacond = Math.Max(tiempo_salida_triage, reloj_operario_reacond);
                        double espera_reacond = inicio_reacond - tiempo_salida_triage;

                        if (inicio_reacond + t_total_reacond > finJornadaMinutos)
                        {
                            cola_reacond++;
                            double espera_restante = finJornadaMinutos - tiempo_salida_triage;
                            if (espera_restante > 0) tiempo_espera_total_minutos += espera_restante;
                        }
                        else
                        {
                            tiempo_espera_total_minutos += espera_reacond;
                            relojes_reacond[indiceOpReacond] = inicio_reacond + t_total_reacond;
                            cant_rec++;
                            temp_borrado += t_borrado;
                            temp_arreglo += t_arreglo;
                            if (isCelular) cel_reacond++; else tab_reacond++;
                        }
                    }
                    else
                    {
                        // MINERÍA (Desmantelamiento)
                        double t_min = dist.Uniforme(25, 40);

                        // CORRECCIÓN PARALELO: Buscamos operario libre en Minería
                        int indiceOpMineria = ObtenerOperarioMasLibre(relojes_mineria);
                        double reloj_operario_mineria = relojes_mineria[indiceOpMineria];

                        double inicio_mineria = Math.Max(tiempo_salida_triage, reloj_operario_mineria);
                        double espera_mineria = inicio_mineria - tiempo_salida_triage;

                        if (inicio_mineria + t_min > finJornadaMinutos)
                        {
                            cola_mineria++;
                            double espera_restante = finJornadaMinutos - tiempo_salida_triage;
                            if (espera_restante > 0) tiempo_espera_total_minutos += espera_restante;
                        }
                        else
                        {
                            tiempo_espera_total_minutos += espera_mineria;
                            relojes_mineria[indiceOpMineria] = inicio_mineria + t_min;
                            cant_min++;
                            temp_min += t_min;
                            total_metales += (peso_actual * 0.35);
                            total_plastico += (peso_actual * 0.40);
                            if (isCelular) cel_desmantelados++; else tab_desmanteladas++;
                        }
                    }
                }
            }

            // CORRECCIÓN PRINCIPAL: Calcular el promedio real dividiendo por la cantidad de equipos ingresados
            int totalEquiposIngresados = contador_celulares + contador_tablets;
            double tiempoPromedioHoras = 0;

            if (totalEquiposIngresados > 0)
            {
                // (Minutos totales / 60) nos da las horas totales, y lo dividimos por la cantidad de equipos
                tiempoPromedioHoras = (tiempo_espera_total_minutos / 60.0) / totalEquiposIngresados;
            }

            // Cálculos de eficiencias individuales basadas en el tiempo total que laburaron los operarios sobre su capacidad instalada
            double totalMinutosDisponiblesTriage = finJornadaMinutos * parametros.EmpleadosTriage;
            double totalMinutosDisponiblesMineria = finJornadaMinutos * parametros.EmpleadosDesmantelamiento;
            double totalMinutosDisponiblesReacond = finJornadaMinutos * parametros.EmpleadosReaciclaje;

            double eficTriage = totalMinutosDisponiblesTriage > 0 ? (TUT / totalMinutosDisponiblesTriage) * 100 : 0;
            double eficMineria = totalMinutosDisponiblesMineria > 0 ? (temp_min / totalMinutosDisponiblesMineria) * 100 : 0;
            double eficReacond = totalMinutosDisponiblesReacond > 0 ? ((temp_borrado + temp_arreglo) / totalMinutosDisponiblesReacond) * 100 : 0;

            var registro = new SimulacionRecord
            {
                TotalEquiposIngresados = totalEquiposIngresados,
                EquiposReacondicionados = cant_rec,
                EquiposDesmantelados = cant_min,
                KilosMetalRecuperado = Math.Round(total_metales, 2),
                KilosPlasticoRecuperado = Math.Round(total_plastico, 2),
                TiempoPromedioEspera = Math.Round(tiempoPromedioHoras, 2), // <-- Guardamos el promedio real corregido
                EficienciaTriage = Math.Round(eficTriage, 2),
                EficienciaDesmantelamiento = Math.Round(eficMineria, 2),
                EficienciaReacondicionamiento = Math.Round(eficReacond, 2),
                EquiposEnColaTriage = cola_triage,
                EquiposEnColaDesmantelamiento = cola_mineria,
                EquiposEnColaReacondicionamiento = cola_reacond,
                KilosBasuraFisicaTotal = Math.Round(total_basura_cruda, 2),
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
                KilosBasuraFisicaTotal = registro.KilosBasuraFisicaTotal,
                CelularesIngresados = registro.CelularesIngresados,
                TabletsIngresadas = registro.TabletsIngresadas,
                CelularesDesmantelados = registro.CelularesDesmantelados,
                TabletsDesmanteladas = registro.TabletsDesmanteladas,
                CelularesReacondicionados = registro.CelularesReacondicionados,
                TabletsReacondicionadas = registro.TabletsReacondicionadas
            };
        }

        // Función auxiliar para simular canales en paralelo. Devuelve el índice del operario más rápido en liberarse.
        private int ObtenerOperarioMasLibre(double[] relojes)
        {
            int indiceMin = 0;
            double valorMin = relojes[0];
            for (int i = 1; i < relojes.Length; i++)
            {
                if (relojes[i] < valorMin)
                {
                    valorMin = relojes[i];
                    indiceMin = i;
                }
            }
            return indiceMin;
        }
    }
}