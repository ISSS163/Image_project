using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Статический класс для хранения различного рода ядер сверток
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/Kernel_(image_processing)</remarks>
    public static class Kernels
    {
        /// <summary>
        /// Гауссово ядро
        /// </summary>
        /// <param name="vSize">Вертикальный размер</param>
        /// <param name="vSigma">СКО по вертикали</param>
        /// <param name="hSize">Горизонтальный размер</param>
        /// <param name="hSigma">СКО по горизонтали</param>
        /// <param name="corr">Коэффициент корреляции</param>
        /// <returns></returns>
        public static float[,] GetGaussianKernel(int vSize, int hSize, float vSigma,  float hSigma ,  float corr)
        {
            if (vSize <= 0 || hSize <= 0)
                throw new ArgumentException("Wrong size");
            if(vSigma < 0 || hSigma< 0 )
                throw new ArgumentException("Wrong sigma value");
            if(corr < 0 || corr > 1)
                throw new ArgumentException("Wrong correlation value");
            float centerV = vSize / 2.0f - 0.5f;
            float centerH = hSize / 2.0f - 0.5f;
            float[,] result = new float[vSize, hSize];
            float sum = 0.0f;

            float a = vSigma * hSigma == 0 
                ? MathF.Sqrt(2 * MathF.PI) * Math.Max(vSigma, hSigma) //одно из СКО = 0 - возврат к одномерному случаю 
                : 2 * MathF.PI * hSigma * vSigma * MathF.Sqrt(1 - corr * corr);

            a = 1 / a;

            for (int i = 0; i < vSize; i++)
                for (int j = 0; j < hSize; j++)
                {
                    float vDist =  MathF.Abs(i - centerV);
                    float hDist =  MathF.Abs(j - centerH);

                    float dist, val;
                    if (vSigma * hSigma == 0) // одно из СКО = 0
                    {
                        dist = vSigma == 0 ? hDist : vDist;
                        float sigma = vSigma == 0 ? hSigma : vSigma;
                        val = -(dist * dist) / (2 * sigma);
                        val = a * MathF.Exp(val);
                    }
                    else
                    {
                        float v = (vDist * vDist) /vSigma;
                        float h = (hDist * hDist) / hSigma;
                        float d = 2*corr*vDist*hDist / (vSigma*hSigma);
                        val = -(v +  h - d) / (1 - corr * corr);
                        val = a * MathF.Exp(val);
                    }
                    sum += val;
                    result[i, j] = val;
                }
            for (int i = 0; i < vSize; i++)
                for (int j = 0; j < hSize; j++)
                    result[i, j] /= sum;
            return result;
        }

        /// <summary>
        /// Квадратное гауссово ядро 
        /// </summary>
        /// <param name="size">Размер</param>
        /// <param name="sigma">СКО</param>
        /// <param name="corr">Коэффициент корреляции</param>
        /// <returns></returns>
        public static float[,] GetGaussianKernel(int size, float sigma=1, float corr = 0)
        {
            return GetGaussianKernel(size, size, sigma, sigma, corr);
        }

        /// <summary>
        /// Ядро, повышающее резкость изображения
        /// </summary>
        /// <returns></returns>
        public static float[,] GetSharpeningKernel()
        {
            throw new NotImplementedException();
        }

        public static float[,] GetUniformKernel(int vSize, int hSize)
        {
            //throw new NotImplementedException();
            float[,] result = new float[vSize,hSize];
            float val = 1.0f / (vSize * hSize);
            for (int i = 0; i < vSize; i++)
                for (int j = 0; j < hSize; j++)
                {
                    result[i, j] = val;
                }
            return result;
        }

        /// <summary>
        /// Ядра оператора Собеля
        /// </summary>
        /// <returns>Ядра для вертикальных и горизонтальных границ</returns>
        public static (float[,], float[,]) GetSobelKernels()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ядро оперератора Лапласа
        /// </summary>
        /// <returns></returns>
        public static float[,] GetLaplaceKernel()
        {
            return new float[,] { 
                { -1, -1, -1 },
                { -1,  8, -1 },
                { -1, -1, -1 }
            };
        }

        /// <summary>
        /// Ядра оператора Щарра
        /// </summary>
        /// <returns>Ядра для вертикальных и горизонтальных границ</returns>
        public static (float[,], float[,]) GetSharrKernels() 
        {
            throw new NotImplementedException(); 
        }

        /// <summary>
        /// Ядра оператора Превитт(Прюитт)
        /// </summary>
        /// <returns>Ядра для вертикальных и горизонтальных границ</returns>
        public static (float[,], float[,]) GetPrewittKernels()
        {
            var kernelV = new float[,]
            {
                {-1,-1,-1 },
                { 0, 0, 0 },
                { 1, 1, 1 }
            };

            var kernelH = new float[,]
            {
                {-1, 0, 1 },
                {-1, 0, 1 },
                {-1, 0, 1 }
            };

            return (kernelV, kernelH);
        }

    }
}
