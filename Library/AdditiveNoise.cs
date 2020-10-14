using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    //PATTERN Шаблонный метод


    /// <summary>
    /// Аддитивный шум
    /// </summary>
    /// <remarks>
    /// В случае аддитивного шума выходное изображение X1 можно предаствить как сумму исходного изображения X0 и шума N
    /// X1 = X0 + N
    /// </remarks>
    public abstract class AdditiveNoise: PixelwiseFilter
    {
        public abstract float GetNoiseValue();

        protected Random _r = new Random();
        protected override void ProcessPixel(Image image, int vPos, int hPos)
        {
            for (int k = 0; k < image.Channels; k++)
            {
                float result = GetNoiseValue();
                image[vPos, hPos, k] = Math.Clamp(result + image[vPos, hPos, k], 0, 255);
            }
        }
    }
}
