using SimulacionBackend.DTOs;
using SimulacionBackend.Generadores;
using System;
using System.Data;

namespace SimulacionBackend.Services
{
    public class SimulacionService : ISimulacionService
    {
        public SimulacionResultDTO EjecutarSimulacion(SimulacionRequestDTO parametros)
        {
            long semilla = DateTime.Now.Ticks % int.MaxValue;
            var congruencial = new CongruencialMixto(semilla, 1664525, 1013904223, (long)Math.Pow(2, 32));
            var dist = new Distribuciones(congruencial);

            int contador_tablets = 0;
            int contador_celulares = 0;
            int cant_min = 0;
            int cant_rec = 0; 
            double TUT = 0, temp_min = 0, temp_borrado = 0, temp_arreglo = 0;
            double total_metales = 0, total_plastico = 0; 

           
            for (int i = 1; i <= parametros.HorasJornada; i++)
            {
                
                int lotesDelDia = dist.Poisson(4);

                for (int j = 1; j <= lotesDelDia; j++)
                {
                    double kgLote = dist.Normal(120, 15);
                    double kg_telTabl = kgLote * 0.08;
                    double peso_acumulado = 0;

                   
                    while (peso_acumulado < kg_telTabl)
                    {
                        TUT += dist.Uniforme(15, 20);
                        double peso_actual = 0;
                        double u = congruencial.obtenerSiguiente();

                        if (u <= 0.7) 
                        {
                            contador_celulares++;
                            peso_actual = dist.Normal(0.200, 0.20); 

                            double u1 = congruencial.obtenerSiguiente();

                            if (u1 <= 0.1) { /* eliminacion */ }
                            else if (u1 <= 0.25)
                            {
                                EjecutarReacondicionamiento(congruencial, dist, ref cant_rec, ref temp_borrado, ref temp_arreglo);
                            }
                            else
                            {
                                EjecutarMineria(dist, peso_actual, ref cant_min, ref temp_min, ref total_metales, ref total_plastico);
                            }
                        }
                        else 
                        {
                            contador_tablets++;
                            peso_actual = dist.Normal(0.700, 0.50); 

                            double u2 = congruencial.obtenerSiguiente();

                            if (u2 <= 0.1) { /* eliminacion */ }
                            else if (u2 <= 0.25)
                            {
                                EjecutarReacondicionamiento(congruencial, dist, ref cant_rec, ref temp_borrado, ref temp_arreglo);
                            }
                            else
                            {
                                EjecutarMineria(dist, peso_actual, ref cant_min, ref temp_min, ref total_metales, ref total_plastico);
                            }
                        }

                        
                        peso_acumulado += peso_actual;
                    }
                }
            }


            double tiempoTotalMinutos = TUT + temp_min + temp_borrado + temp_arreglo;
            double tiempoTotalHoras = tiempoTotalMinutos / 60.0;

            return new SimulacionResultDTO
            {
                TotalEquiposIngresados = contador_celulares + contador_tablets,
                EquiposReacondicionados = cant_rec,
                EquiposDesmantelados = cant_min,
                KilosMetalRecuperado = Math.Round(total_metales, 2),
                KilosPlasticoRecuperado = Math.Round(total_plastico, 2),
                TiempoPromedioEspera = Math.Round(tiempoTotalHoras, 2) // Ahora sí mandamos el dato
            };
        }

       
        private void EjecutarMineria(Distribuciones dist, double peso_actual, ref int cant_min, ref double temp_min, ref double total_metales, ref double total_plastico)
        {
            cant_min++;
            temp_min += dist.Uniforme(25, 40);
            total_metales += (peso_actual * 0.35);
            total_plastico += (peso_actual * 0.40);
        }

        private void EjecutarReacondicionamiento(CongruencialMixto congruencial, Distribuciones dist, ref int cant_rec, ref double temp_borrado, ref double temp_arreglo)
        {
            cant_rec++;
            temp_borrado += dist.Uniforme(30, 45);

            double t_arreglo = congruencial.obtenerSiguiente();
            if (t_arreglo <= 0.65)
            {
                temp_arreglo += dist.Exponencial(90);
            }
            else
            {
                temp_arreglo += dist.Exponencial(180);
            }
        }
    }
}