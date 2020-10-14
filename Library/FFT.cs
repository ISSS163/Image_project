using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Library
{
    /// <summary>
    /// Быстрое преобразование Фурье
    /// </summary>
    static class FFT
    {
        static void ReorderRow(Complex[,] data, int row) //найдено на просторах Интернета
        {
            Complex tmp;
            int N = data.GetLength(1);
            int p = Math.ILogB(N);
            for (int n = 0; n < N; n++)
            {
                int j = 0;
                int m = n;
                for (int i = 0; i < p; i++)
                {
                    j = 2*j + m % 2;
                    m = m / 2;
                }

                if (j > n)
                {
                    tmp = data[row, j];
                    data[row, j] = data[row, n];
                    data[row, n] = tmp;
                }

            }
        }

        static void ReorderColumn(Complex[,] data, int column)
        {
            Complex tmp;
            int N = data.GetLength(0);
            int p = Math.ILogB(N);
            for (int n = 0; n < N; n++)
            {
                int j = 0;
                int m = n;
                for (int i = 0; i < p; i++)
                {
                    j = 2 * j + m % 2;
                    m = m / 2;
                }

                if (j > n)
                {
                    tmp = data[ j, column];
                    data[ j, column] = data[ n, column];
                    data[n, column] = tmp;
                }

            }
        }

        static void ExecuteRow(Complex[,] data, int row, int start, int end, bool isForward)
        {
            if(end - start == 2)
            {
                var x0 = data[row, start];
                var x1 = data[row, end-1];
                data[row, start] = x0 + x1;
                data[row, end-1] = x0 - x1;
                return;
            }


            var N = (end - start );

            var center = start+N / 2;
            ExecuteRow(data, row, start, center, isForward);
            ExecuteRow(data, row, center, end, isForward);

            var multBase = isForward ?  Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne / N) : Complex.Exp(2 * Math.PI * Complex.ImaginaryOne / N);
            //double norm = isForward ? 1 : (1.0 / N);
            for (int k = 0; k < N/2; k++)
            {
                var mult = Complex.Pow(multBase, k);

                var x0 = data[row, start + k];
                var x1 = data[row, center + k];

                data[row, start + k] = (x0 + mult * x1);
                data[row, center + k] = (x0 - mult * x1);
            }


        }

        static void ExecuteColumn(Complex[,] data, int column, int start, int end, bool isForward)
        {
            if (end - start == 2)
            {
                var x0 = data[start, column];
                var x1 = data[end-1, column];
                data[start, column] = x0 + x1;
                data[end-1, column] = x0 - x1;
                return;
            }

            var N = (end - start);

            var center =start+ N / 2;
            ExecuteColumn(data, column, start, center, isForward);
            ExecuteColumn(data, column, center, end, isForward);

            var multBase = isForward ? Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne / N) : Complex.Exp(2 * Math.PI * Complex.ImaginaryOne / N);
            //double norm = isForward ? 1 : (1.0 / N);
            for (int k = 0; k < N / 2; k++)
            {
                var mult = Complex.Pow(multBase, k);

                var x0 = data[start + k, column];
                var x1 = data[center + k, column];

                data[start + k, column] = (x0 + mult * x1);
                data[center + k, column] = (x0 - mult * x1);
            }

        }

        public static bool IsPowerOfTwo(int x)
        {
            return ((x - 1) & x) == 0; //догадайтесь, почему?)  (см. двоичный оператор &)
        }

        /// <summary>
        /// Прямое преобразование Фурье
        /// </summary>
        /// <param name="data"></param>
        public static void Forward(Complex[,] data)
        {
            int height = data.GetLength(0);
            int width = data.GetLength(1);

            if (!IsPowerOfTwo(height) || !IsPowerOfTwo(width))
                throw new ArgumentException("Size of the image must be the power of two");

            //Двумерное преобразование Фурье сводится к одномерным 
            for (int i = 0; i < height; i++)
            {
                ReorderRow(data, i);
                ExecuteRow(data, i, 0, width, true);
            }
            for (int j = 0; j < data.GetLength(1); j++)
            {
                ReorderColumn(data, j);
                ExecuteColumn(data, j, 0, height, true);
            }
        }

        /// <summary>
        /// Обратное преобразование Фурье
        /// </summary>
        /// <param name="data"></param>
        /// <remarks>  </remarks>
        public static void Backward(Complex[,] data)
        {
            int height = data.GetLength(0);
            int width = data.GetLength(1);

            if (!IsPowerOfTwo(height) || !IsPowerOfTwo(width))
                throw new ArgumentException("Size of the image must be the power of two");

            
            for (int j = 0; j < width; j++)
            {
                ReorderColumn(data, j);
                ExecuteColumn(data, j, 0, height, false);
            }
            for (int i = 0; i < height; i++)
            {
                ReorderRow(data, i);
                ExecuteRow(data, i, 0, width, false);
            }
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    data[i, j] /= (data.Length);
        }
    }
}
