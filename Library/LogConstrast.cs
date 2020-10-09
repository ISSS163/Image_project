using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Логарифмическое масштабирование
    /// </summary>
    /// <remarks>
    /// Иногда значения пикселей имеют очень большой размах, например (0,1000).
    /// Вполне можно использовать логарифм для уменьшения динамического диапазона.
    /// Например, (0,1000) => (0, 6.9)
    /// </remarks>
    public class LogConstrast : PixelwiseFilter
    {
        protected override void ProcessPixel(Image image, int vPos, int hPos)
        {
            for (int k = 0; k < image.Channels; k++)
            {
                var value = image[vPos, hPos, k];
                if (MathF.Abs(value) >  1)
                {
                    float newValue = MathF.Sign(value) * MathF.Log(MathF.Abs(value));
                    image[vPos, hPos, k] = newValue;
                }
            }
        }
    }
}
