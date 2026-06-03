using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulacionBackend.Generadores
{
    public class PruebasEstadisticas
    {
        
        public static bool PruebaPromedios(List<double> numerosAleatorios)
        {
            int n = numerosAleatorios.Count;
            double promedio = numerosAleatorios.Average();

           
            
            double z0 = (promedio - 0.5) * Math.Sqrt(12 * n);

            
            double valorCritico = 1.96;

            Console.WriteLine("=== PRUEBA DE LOS PROMEDIOS ===");
            Console.WriteLine($"Promedio de la muestra: {promedio:F4} (Esperado: 0.5)");
            Console.WriteLine($"Estadístico Z0: {z0:F4}");
            Console.WriteLine($"Rango de aceptación: [-{valorCritico} ; {valorCritico}]");

            if (Math.Abs(z0) <= valorCritico)
            {
                Console.WriteLine("RESULTADO: APROBADO. La media poblacional es estadísticamente válida.\n");
                return true;
            }
            else
            {
                Console.WriteLine("RESULTADO: RECHAZADO. Falla la prueba de los promedios.\n");
                return false;
            }
        }

       
        public static bool PruebaFrecuencia(List<double> numerosAleatorios, int cantidadIntervalos = 10)
        {
            int n = numerosAleatorios.Count;
            double frecuenciaEsperada = (double)n / cantidadIntervalos;
            int[] frecuenciasObservadas = new int[cantidadIntervalos];

            foreach (var num in numerosAleatorios)
            {
                int indice = (int)(num * cantidadIntervalos);
                if (indice == cantidadIntervalos) indice--;
                frecuenciasObservadas[indice]++;
            }

            double estadisticoChi = 0;
            for (int i = 0; i < cantidadIntervalos; i++)
            {
                estadisticoChi += Math.Pow(frecuenciasObservadas[i] - frecuenciaEsperada, 2) / frecuenciaEsperada;
            }

           
            double valorCriticoTabla = 16.919;

            Console.WriteLine("=== PRUEBA DE LA FRECUENCIA (Chi-Cuadrada) ===");
            Console.WriteLine($"Estadístico calculado: {estadisticoChi:F4}");
            Console.WriteLine($"Valor crítico (Tabla): {valorCriticoTabla}");

            if (estadisticoChi <= valorCriticoTabla)
            {
                Console.WriteLine("RESULTADO: APROBADO. Los números siguen una distribución Uniforme.\n");
                return true;
            }
            else
            {
                Console.WriteLine("RESULTADO: RECHAZADO. Falla la prueba de frecuencia.\n");
                return false;
            }
        }
    }
}