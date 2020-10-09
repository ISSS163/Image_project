using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Импульсный шум
    /// </summary>
    /// <remarks>
    /// Шум полностью "выбивает" пиксель (делает его белым).
    /// Нечто похожее можно видеть на старых фотографиях - белые точки, разбросанные по изображению.
    /// </remarks>
    public class ImpulseNoise: PixelwiseFilter
    {

        /// <summary>
        /// Импульсный шум
        /// </summary>
        /// <param name="p">Вероятность зашумления пикселя</param>
        public ImpulseNoise(float p)
        {
            P = p;
        }

        /// <summary>
        /// Вероятность зашумления пикселя
        /// </summary>
        public float P { get; }
        Random r = new Random();

        protected override void ProcessPixel(Image image, int vPos, int hPos)
        {
            if (r.NextDouble() > P)
                return;
            for (int k = 0; k < image.Channels; k++)
            {
                image[vPos, hPos, k] = 255;
            }
        }
    }
}
