namespace SimulacionBackend.Generadores
{
    public class CongruencialMixto 
    {
        private long _xn;
        private readonly long _a;
        private readonly long _c;
        private readonly long _m;

        public CongruencialMixto(long semilla, long a, long c, long m)
        {
            _xn = semilla;
            _a = a;
            _c = c;
            _m = m;
        }

        public double obtenerSiguiente()
        {
            _xn = (_a * _xn + _c) % _m;
            return (double)_xn / _m;
        }
    }
}
