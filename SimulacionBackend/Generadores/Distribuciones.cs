namespace SimulacionBackend.Generadores
{
    public class Distribuciones
    {
        private readonly CongruencialMixto _generador;

        public Distribuciones(CongruencialMixto generador) 
        {
            _generador = generador;
        }

        public double Uniforme(double a, double b)
        {
            double u = _generador.obtenerSiguiente();
            return a + (b - a) * u;
        }

        public double Exponencial(double lambda)
        {
            double u = _generador.obtenerSiguiente();
            return -lambda * Math.Log(u);
        }

        public double Normal(double media, double desviacionEstandar)
        {
            double u1 = _generador.obtenerSiguiente();
            double u2 = _generador.obtenerSiguiente();

            //para no usar bibliotecas propias de estadistica, se utiliza el método de Box-Muller para generar una variable aleatoria normal(Z0) este metodo utiliza dos numeros entre 0 y 1 para generar una normal .(IA no tengo un cpu en el anco)

            double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

            return media + z0 * desviacionEstandar;
        }

        public int Poisson(double lambda)
        {
            double limite = Math.Exp(-lambda);

            double p = 1.0;
            int k = 0;

            do
            {
                k++;
                double u = _generador.obtenerSiguiente();
                p = p * u;
            }while(p > limite);
            return k - 1; //se resta 1 porque el ciclo se ejecuta una vez más después de que p cae por debajo del límite (aclaro por que por ahi no se acuerdan como funciona un do while cabezas de pn)
        }
    }
}
