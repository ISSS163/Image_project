using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Гауссовский шум
    /// </summary>
    public class GaussianNoise : AdditiveNoise
    {
        /// <summary>
        /// Гауссовский шум
        /// </summary>
        /// <param name="m">Мат.ожидание</param>
        /// <param name="sigma">СКО</param>
        public GaussianNoise(float sigma, float m = 0)
        {
            if (sigma <= 0)
                throw new ArgumentException("Sigma <= 0");
            this.M = m;
            this.Sigma = sigma;
        }

        /// <summary>
        /// Мат. ожидание
        /// </summary>
        public float M { get; }

        /// <summary>
        /// СКО
        /// </summary>
        public float Sigma { get; }

        //преобразование Бокса-Мюллера - из равномерного шума (class Random) получает гауссовский
        public override float GetNoiseValue()
        {
            double a = _r.NextDouble();
            double phi = _r.NextDouble();

            double z = 2 * Math.Cos(Math.PI * phi) * Math.Sqrt(-2 * Math.Log(a));
            return (float)(Sigma * z) + M;
        }
    }
}
