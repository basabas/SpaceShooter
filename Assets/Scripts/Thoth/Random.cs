using System;

namespace Bas.Thoth
{
    public static class Random
    {
        private static System.Random _random;
        static Random() => _random = new System.Random((int)DateTime.Now.Ticks);

        public static float Range(float minInclusive, float maxInclusive) => Math.Lerp(minInclusive, maxInclusive, (float)_random.NextDouble());

        public static int Range(int minInclusive, int maxExclusive) => Math.Lerp(minInclusive, maxExclusive, (float)_random.NextDouble());
    }
}