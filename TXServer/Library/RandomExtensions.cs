using System;

namespace TXServer.Library
{
    public static class RandomExtensions
    {
        public static double NextGaussian(this Random random, double mean = 0, double deviation = 1)
        {
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();

            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            // Source: https://stackoverflow.com/a/1303406
            return mean + deviation / 3 * randStdNormal;
        }

        public static double NextGaussianRange(this Random random, double minValue, double maxValue)
        {
            double mean = (minValue + maxValue) / 2;
            return random.NextGaussian(mean, Math.Abs(maxValue - minValue) / 2);
        }
    }
}
