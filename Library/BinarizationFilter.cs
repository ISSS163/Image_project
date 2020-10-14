using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Бинаризация изображения по порогу
    /// </summary>
    /// <remarks>
    /// Пиксели больше заданного порога = 255
    /// Пиксели меньше заданного порога = 0
    /// </remarks>
    public class BinarizationFilter : PixelwiseFilter
    {
        public BinarizationFilter(float edge)
        {
            Edge = edge;
        }

        public float Edge { get; }

        protected override void ProcessPixel(Image image, int vPos, int hPos)
        {
            for (int k = 0; k < image.Channels; k++)
            {
                image[vPos, hPos, k] = image[vPos, hPos, k] < Edge ? 0 : 255;
            }
        }
    }
}
