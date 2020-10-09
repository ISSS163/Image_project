using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Класс, осуществляющий простое контрастирование изображений
    /// </summary>
    public class Contrast : IFilter
    {
        /// <summary>
        /// Контрастирование
        /// </summary>
        /// <param name="max">Верхняя грань выходного(!) интервала яркостей изображения</param>
        /// <param name="min">Нижняя грань выходного(!) интервала яркостей изображения</param>
        /// <remarks>
        /// Поскольку значения пикселей имеют тип float, можно задавать любые интервалы яркостей.
        /// Например, можно создать Contrast(1024, -1024) - это позволит лучше рассмотреть слабые перепады яркости.
        /// </remarks>
        public Contrast(float max=255, float min = 0)
        {
            if (max <= min)
                throw new ArgumentException("Max <= min");
            Min = min;
            Max = max;
        }

        public float Min { get; }
        public float Max { get; }

        public void Apply(Image image)
        {
            Common.ThrowIfNull(image, nameof(image));

            for (int k = 0; k < image.Channels; k++) 
            {
                float max = 0.0f;
                float min = float.MaxValue;
                //считаем минимум и максимум для канала
                for (int i = 0; i < image.Height; i++)
                    for (int j = 0; j < image.Width; j++)
                    {
                        min = Math.Min(min, image[i, j, k]);
                        max = Math.Max(max, image[i, j, k]);
                    }
                //определяем множитель и смещение, чтобы изменить интервал яркостей канала с [min, max] на [this.Min, this.Max]
                float scale, shift;
                if(min == max) //если изображение содержит только один цвет - у нас есть только смещение.
                {
                    scale = 0;
                    shift = max/(Max - Min)+Min;
                }
                else
                {
                    scale = (Max - Min) / (max - min);
                    shift = Min;
                }
                for (int i = 0; i < image.Height; i++)
                    for (int j = 0; j < image.Width; j++)
                        image[i, j, k] = (image[i, j, k] - min) * scale + shift; //смещаем значение пикселя
            }

        }
    }
}
