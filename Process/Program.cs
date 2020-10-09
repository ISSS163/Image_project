using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using Library;

namespace Process
{
    static class FilterNames
    {
        public const string FILTER_GAUSSIAN_BLUR    = "GaussianBlur";
        public const string FILTER_BLUR             = "Blur";
        public const string FILTER_UNIFORM_NOISE    = "UniformNoise";
        public const string FILTER_GAUSSIAN_NOISE   = "GaussianNoise";
        public const string FILTER_LAPLACE          = "Laplace";
        public const string FILTER_PREWITT          = "Prewitt";
        public const string FILTER_CONTRAST         = "Contrast";
        public const string FILTER_LOG_CONTRAST     = "LogContrast";
        public const string FILTER_HSV              = "HSV";
        public const string FILTER_HSV2RGB          = "HSV2RGB";
        public const string FILTER_YCbCR            = "YCbCr";
        public const string FILTER_YCbCR2RGB        = "YCbCr2RGB";
        public const string FILTER_SHARP = "Sharp";
        //TODO: MedianFilter, BinarizationFilter, GrayScale, Sharr, Sobel
    }

    class Program
    {
        const string FILTER_MARK = "-f";
        const string HELP_MARK = "-h";

        static bool ParseInt(string s, out int x)
        {
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out x);
        }

        static bool ParseFloat(string s, out float x)
        {
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out x);
        }

        static IFilter CreateFilter(string filterName, List<string> args)
        {
            //количество аргументов
            int argsCount = args == null ? 0 : args.Count;

            switch (filterName)
            {
                case FilterNames.FILTER_BLUR:
                    {
                        int size = 3; //параметр по умолчанию
                        if (argsCount > 0)
                        {
                            if (!ParseInt(args[0], out size) || size <= 1)
                                throw new ArgumentException("Blur: Invalid size");
                        }
                        return new ConvolutionalFilter(Kernels.GetUniformKernel(size, size));
                    }
                case FilterNames.FILTER_GAUSSIAN_BLUR:
                    {
                        float sigma = 1; //параметры по умолчанию
                        int size = 3;
                        if (argsCount > 0)
                        {
                            if (!ParseInt(args[0], out size) || size <= 1)
                                throw new ArgumentException("Gaussian Blur: Invalid size");
                        }
                        if (argsCount > 1)
                        {
                            if (!ParseFloat(args[1], out sigma) || sigma <= 0)
                                throw new ArgumentException("Gaussian Blur: Invalid StdDev");
                        }
                        return new ConvolutionalFilter(Kernels.GetGaussianKernel(size, sigma));
                    }
                case FilterNames.FILTER_UNIFORM_NOISE:
                    {
                        int amplitude = 5;
                        if (argsCount > 0)
                        {
                            if (!ParseInt(args[0], out amplitude) || amplitude < 1)
                                throw new ArgumentException("Uniform Noise: Invalid amplitude");
                        }
                        return new UniformNoise(amplitude);
                    }
                case FilterNames.FILTER_GAUSSIAN_NOISE:
                    {
                        float sigma = 0.15f;
                        if (argsCount > 0)
                        {
                            if (!ParseFloat(args[0], out sigma) || sigma < 0)
                                throw new ArgumentException("Gaussian Noise: Invalid StdDiv (must be > 0)");
                        }
                        return new GaussianNoise(sigma);
                    }
                case FilterNames.FILTER_LAPLACE:
                    {
                        return new EdgeDetection(EdgeDetection.EdgeDetectionAlgorithm.Laplace);
                    }
                case FilterNames.FILTER_PREWITT:
                    {
                        return new EdgeDetection(EdgeDetection.EdgeDetectionAlgorithm.Prewitt);
                    }
                case FilterNames.FILTER_CONTRAST:
                    {
                        float maxValue = 255;
                        float minValue = 0;

                        if (argsCount > 0)
                        {
                            if (!ParseFloat(args[0], out maxValue))
                                throw new ArgumentException("Contrast: Invalid max value");
                        }
                        if (argsCount > 1)
                        {
                            if (!ParseFloat(args[1], out minValue) || minValue >= maxValue)
                                throw new ArgumentException("Contrast: Invalid min value");
                        }
                        return new Contrast(maxValue, minValue);
                    }
                case FilterNames.FILTER_SHARP:
                    {
                        return new ConvolutionalFilter(Kernels.GetSharpeningKernel());
                    }
                case FilterNames.FILTER_LOG_CONTRAST:
                    {
                        return new LogConstrast();
                    }
                case FilterNames.FILTER_HSV:
                    {
                        return new ColorSpaceConversion(ColorSpaceConversion.ConversionType.RGB2HSV);
                    }
                case FilterNames.FILTER_HSV2RGB:
                    {
                        return new ColorSpaceConversion(ColorSpaceConversion.ConversionType.HSV2RGB);
                    }
                case FilterNames.FILTER_YCbCR:
                    {
                        return new ColorSpaceConversion(ColorSpaceConversion.ConversionType.RGB2YCbCr);
                    }
                case FilterNames.FILTER_YCbCR2RGB:
                    {
                        return new ColorSpaceConversion(ColorSpaceConversion.ConversionType.YCbCr2RGB);
                    }

                //TODO: MedianFilter, BinarizationFilter, ImpulseNoise, GrayScale, Sharr, Sobel


                default:
                    throw new ArgumentException("Unsupported filter");
            }
        }

        static List<IFilter> ParseArgs(string[] args)
        {
            int pos = 0;
            int nextPos = 0;
            List<IFilter> result = new List<IFilter>();
            while (pos < args.Length - 2) // последние 2 аргумента - это входной и выходной файлы
            {
                if (args[pos] == FILTER_MARK) // если текущий элемент - флаг фильтра (-f)
                {
                    for (nextPos = pos + 1; nextPos < args.Length - 2 && args[nextPos] != FILTER_MARK; nextPos++) ; // ищем следующий флаг (-f Contrast ... -f[<=] )
                    if (nextPos == pos + 1) 
                        throw new ArgumentException("Incorrect arguments");//не бывает 2 флагов подряд
                    string filterName = args[pos + 1]; // имя фильтра - следующий аргумент (-f Contrast[<=] ... )
                    List<string> filterArgs = new List<string>();
                    for (int i = pos + 2; i < nextPos; i++) // все, что идет до следующего флага - это аргументы фильтра  (-f Contrast 255[<=] 0[<=] -f)
                    {
                        filterArgs.Add(args[i]);
                    }
                    result.Add(CreateFilter(filterName, filterArgs));
                    pos = nextPos;
                }
                else ++pos;
            }
            return result;
        }

        const string Help = 
@"This program can be used to process images using different filters.
Usage: Process [-f Name Params...] {Input} {Output}
Params:
    Name   - Name of the filter,
    Params - List of filter params delimeted by spaces.
    Input  - Path to the input image.
    Output - Path to the output image (only .png and .jpeg/.jpg are supported).

There can be more than 1 filters applied. In this case, the filters will be applied sequentially.

Example:
    Process -f UniformNoise 10 -f Blur 5 1.bmp 1.png
";


        static void Main(string[] args)
        {
            if (args.Length < 3) { //Нет аргументов
                Console.WriteLine("Error: no arguments. Printing help.");
                Console.WriteLine();
                Console.WriteLine(Help);
                return;
                //args = new[] { "-f", "Laplace", "-f", "YCbCr2RGB",  @"D:\WORK\Data\Tmp\IN\0.bmp", @"D:\WORK\Data\Tmp\OUT\0.png" };
            }


            for(int i = 0; i < args.Length; i++)
                if(args[i] == HELP_MARK) //если любой аргумент = '-h' - напечатать help и выйти.
                {
                    Console.WriteLine(Help);
                    return;
                }

            string inputFile = args[args.Length - 2]; //имя входного файлы - предпоследний аргумент
            string outputFile = args[args.Length - 1]; //имя вЫходного файла - последний

            List<IFilter> filters;
            try
            {
                filters = ParseArgs(args);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error while parsing arguments: " + e.Message);
                return;
            }

            try
            {
                Image image = Image.Open(inputFile);

                for (int i = 0; i < filters.Count; i++)
                {
                    filters[i].Apply(image);
                }

                image.Save(outputFile);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return;
            }
        }
    }
}
