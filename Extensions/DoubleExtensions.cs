using System;

namespace simulation
{
    public static class DoubleExtensions
    {
        public static double Round(this double x, int decimals = 2)
        {
            return Math.Round(x, decimals);
        }
    }

}
