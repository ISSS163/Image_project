using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    //PATTERN Шаблонный метод

    /// <summary>
    /// Равномерный шум с заданной амплитудой
    /// </summary>
    public class UniformNoise : AdditiveNoise
    {
        /// <summary>
        /// Равномерный шум с заданной амплитудой
        /// </summary>
        /// <param name="amplitude">Амплитуда шума</param>
        public UniformNoise(int amplitude)
        {
            Amplitude = (amplitude > 0 & amplitude <= 255) ? amplitude : throw new ArgumentException("Invalid noise amplitude");
        }


        /// <summary>
        /// Амплитуда шума
        /// </summary>
        public int Amplitude { get; }

        public override float GetNoiseValue()
        {
            return _r.Next(-Amplitude / 2, Amplitude / 2 + Amplitude % 2 + 1);
        }
    }
}
