using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Library
{
    //PATTERN Фасад

    /// <summary>
    /// Частотный фильтр
    /// </summary>
    public class FrequencyFilter : IFilter
    {
        public enum FilterMode
        {
            HighPass,
            LowPass
        }


        public FrequencyFilter(FilterMode m, float s)
        {
            if (s < 0 || s > 1)
                throw new ArgumentException("High-frequency share shall be in [0, 1]");
            Mode = m;
            S = s;
        }

        public FilterMode Mode { get; }
        public float S { get; }

        static int NextPow2(int x)
        {
            if (FFT.IsPowerOfTwo(x))
                return x;
            return 1 << (Math.ILogB(x) + 1);
        }

        static void Cast(Image input, Complex[,] output, int channel)
        {
            
            for (int i = 0; i < output.GetLength(0); i++)
            {
                for (int j = 0; j < output.GetLength(1); j++)
                {
                    output[i, j] = input[i % input.Height, j % input.Width, channel];
                }
            }
        }

        static void Cast(Complex[,] input,Image output,  int channel)
        {
            for (int i = 0; i < output.Height; i++)
            {
                for (int j = 0; j < output.Width; j++)
                {
                    output[i, j, channel] = (float)input[i, j].Magnitude;
                }
            }
        }

        public void Apply(Image img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            Complex[,] result = new Complex[NextPow2(img.Height), NextPow2(img.Width)];

            for (int k = 0; k < img.Channels; k++)
            { 
                Cast(img, result, k);//Refill array with 0
                FFT.Forward(result);
                this.Filter(result);
                FFT.Backward(result);
                Cast(result, img, k);
            }
        }

        private void Filter(Complex[,] result)
        {
            var s = Mode == FilterMode.LowPass ? Math.Sqrt(1-this.S) :  Math.Sqrt(this.S);


            int width = result.GetLength(0), height = result.GetLength(1);

            int fWidth = (int)Math.Round(width * s);
            int fHeight = (int)Math.Round(height * s);

            int hGap = (width - fWidth) / 2;
            int vGap = (height - fHeight) / 2;



            if (Mode == FilterMode.LowPass)
            {

                for (int i = vGap; i < height - vGap; i++)
                    for (int j = hGap; j < width - hGap; j++)
                    {
                        result[i, j] = 0;
                    }
            }
            else
            {
                //if (vGap < 1 || hGap < 1)
                //{
                //    for (int i = 0; i < height; i++)
                //        for (int j = hGap; j < width; j++)
                //        {
                //            result[i, j] = 0;
                //        }
                //}
                for (int i = 0; i < vGap; i++)
                    for (int j = 0; j < width; j++)
                    {
                        result[i, j]
                            = result[height - i - 1, j] = 0;
                    }

                for (int i = vGap; i <= height - vGap; i++)
                    for (int j = 0; j < hGap; j++)
                    {
                        result[i, j]
                            = result[i, width - j - 1] = 0;
                    }
            }
        }


    }
}
