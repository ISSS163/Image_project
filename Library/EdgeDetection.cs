using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Метод выделения границ
    /// </summary>
    public class EdgeDetection: WindowedFilter 
    {
        public enum EdgeDetectionAlgorithm
        {
            /// <summary>
            /// Прюитт (Превитт)
            /// </summary>
            Prewitt,
            /// <summary>
            /// Собель
            /// </summary>
            Sobel,
            /// <summary>
            /// Щарр
            /// </summary>
            Sharr,
            /// <summary>
            /// Лаплас (фильтр второго порядка)
            /// </summary>
            Laplace
        }

        private static (float[,],float[,]) GetKernels(EdgeDetectionAlgorithm alg)
        {
            switch (alg)
            {
                case EdgeDetectionAlgorithm.Sobel:
                    return Kernels.GetSobelKernels();
                case EdgeDetectionAlgorithm.Prewitt:
                    return Kernels.GetPrewittKernels();
                case EdgeDetectionAlgorithm.Sharr:
                    return Kernels.GetSharrKernels();
                case EdgeDetectionAlgorithm.Laplace:
                    return (Kernels.GetLaplaceKernel(), null); //Оператор Лапласа использует 1 ядро свертки
                default:
                    throw new ArgumentException("Unknown edge detection algorithm");
            }
        }

        #region answer_2
        //HSV
        #endregion

        public EdgeDetection(EdgeDetectionAlgorithm alg): base(3, 3)
        {
            (KernelV, KernelH) = GetKernels(alg);
        }

        /// <summary>
        /// Ядро для выделения горизонтальных границ
        /// </summary>
        public float[,] KernelH { get; set; }

        /// <summary>
        /// Ядро для выделения вертикальных границ
        /// </summary>
        public float[,] KernelV { get; set; }

        protected override void ProcessWindow(Image inImage, Image outImage, int vPos, int hPos)
        {
            for (int k = 0; k < outImage.Channels; k++)
            {
                float resultV = Common.Convolve(inImage, vPos, hPos,k, KernelV);
                float resultH = 0;
                if(KernelH != null) //некоторые операторы используют только 1 ядро свертки
                {
                    resultH = Common.Convolve(inImage, vPos, hPos, k, KernelH);
                }
                outImage[vPos, hPos, k] =  (float)Math.Sqrt(resultH * resultH + resultV * resultV);
            }

        }
    }
}
