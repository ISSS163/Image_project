using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    //PATTERN Шаблонный метод

    /// <summary>
    /// Фильтр попиксельной обработки
    /// </summary>
    public abstract class PixelwiseFilter: IFilter
    {
        public void Apply(Image image)
        {
            Common.ThrowIfNull(image, nameof(image));
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                    ProcessPixel(image, i, j);
        }

        protected abstract void ProcessPixel(Image image, int vPos, int hPos);
    }
}
