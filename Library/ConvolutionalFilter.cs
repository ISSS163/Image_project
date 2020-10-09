using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Library
{
    /// <summary>
    /// Сверточный фильтр
    /// </summary>
    public class ConvolutionalFilter: WindowedFilter 
    { 
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="kernel">Ядро свертки размера MxN, где M,N - нечетные числа</param>
        public ConvolutionalFilter(float[,] kernel) : base(kernel?.GetLength(0) ?? 1, kernel?.GetLength(1) ?? 1)
        {
            Common.ThrowIfNull(kernel, nameof(kernel));
            Kernel = kernel;
        }

        /// <summary>
        /// Ядро свертки
        /// </summary>
        public float[,] Kernel { get; }

        protected override void ProcessWindow(Image inImage, Image outImage, int vPos, int hPos)
        {
            for (int k = 0; k < outImage.Channels; k++)
            {
                outImage[vPos, hPos, k] = Common.Convolve(inImage, vPos, hPos, k, this.Kernel);
            }
        }
    }
}
