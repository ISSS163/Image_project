using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Медианный фильтр
    /// </summary>
    /// <remarks>
    /// Эффективно борется с импульсным шумом.
    /// </remarks>
    public class MedianFilter : WindowedFilter
    {
        /// <summary>
        /// Медианный фильтр
        /// </summary>
        /// <param name="size">Размер фильтра (окно SIZExSIZE)</param>
        public MedianFilter(int size) : base(size, size) {}


        /// <summary>
        /// метод обработки окна
        /// </summary>
        /// <param name="inImage"></param>
        /// <param name="outImage"></param>
        /// <param name="vPos"></param>
        /// <param name="hPos"></param>
        protected override void ProcessWindow(Image inImage, Image outImage, int vPos, int hPos)
        {
            //коллекция пар (ключ, значение) с автоматической сортировкой по ключу
            SortedList<float, object> values = new SortedList<float, object>();

            for (int k = 0; k < outImage.Channels; k++)
            {
                for (int i = 0; i < this.WindowHeight; i++)
                    for (int j = 0; j < this.WindowWidth; j++)
                        values.Add(inImage[vPos + i, hPos + j, k], null);

                float median = values.Keys[values.Count / 2]; //ключи коллекции отсортированы, медиана находится в середине
                outImage[vPos, hPos, k] = median;
                values.Clear();
            }
        }
    }
}
