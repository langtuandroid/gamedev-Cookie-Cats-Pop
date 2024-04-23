using System;

namespace TactileModules.RuntimeTools.Random
{
    public class SeededRandom : IRandom
    {
        public SeededRandom(int seed)
        {
            this.random = new System.Random(seed);
        }

        public float GetNextFloat()
        {
            return (float)this.random.NextDouble();
        }

        public int Range(int minValue, int maxValue)
        {
            return this.random.Next(minValue, maxValue);
        }

        private System.Random random;
    }
}
