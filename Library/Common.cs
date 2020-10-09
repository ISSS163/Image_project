using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    static class Common
    {
        /// <summary>
        /// Свертка
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="vPos">Вертикальная позиция левого нижнего угла текущего окна (не центра!)</param>
        /// <param name="hPos">Горизонтальная позиция левого нижнего угла текущего окна (не центра!)</param>
        /// <param name="channel">Текущий канал изображения</param>
        /// <param name="kernel">Ядро свертки</param>
        /// <returns></returns>
        public static float Convolve(Image image, int vPos, int hPos, int channel, float[,] kernel)
        {
            Common.ThrowIfNull(image, nameof(image));
            Common.ThrowIfNull(kernel, nameof(kernel));
            float result = 0;

            int windowHeight = kernel.GetLength(0);
            int windowWidth = kernel.GetLength(1);

            //см. выражение для двумерной свертки
            for (int i = 0; i < windowHeight; i++)
                for (int j = 0; j < windowWidth; j++)
                {
                    result += image[vPos + i, hPos + j, channel] * kernel[windowHeight - i - 1, windowWidth - j - 1];
                }
            return result;
        }

        public static void ThrowIfNull(object arg, string name)
        {
            if (arg == null)
                throw new ArgumentNullException(name);
        }

        #region ANSWER3
        /*
         *  Contrast 10000
            Prewitt
            LogContrast
            Contrast
         */
        #endregion

    }
}
