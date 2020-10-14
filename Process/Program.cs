using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using Library;
using SkiaSharp;

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
        public const string FILTER_SHARP            = "Sharp";
        public const string FILTER_MEDIAN           = "Median";
        public const string FILTER_BINARIZATION     = "Binarize";
        public const string FILTER_GRAYSCALE        = "Gray";
        public const string FILTER_SHARR            = "Sharr";
        public const string FILTER_SOBEL            = "Sobel";
        public const string FILTER_IMPULSE_NOISE    = "ImpulseNoise";
        public const string FILTER_HIGH_PASS        = "HighPass";
        public const string FILTER_LOW_PASS         = "LowPass";      
    }

    class Program
    {
        const string FILTER_MARK = "-f";
        const string HELP_MARK = "-h";

        static bool Parse<T>(string s, out T x)
        {
            bool result = false ;
            if (typeof(T) == typeof(float))
            {
                float val;
                result = float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out val);
                x = (T)Convert.ChangeType(val, typeof(T));
            }
            else if (typeof(T) == typeof(int))
            {
                int val;
                result = int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out val);
                x = (T)Convert.ChangeType(val, typeof(T));
            }
            else
                throw new NotSupportedException("Unsupported type");
            return result;
        }

        static T ParseArg<T>(List<string> args, int pos, T defaultValue, string errorMsg)
        {
            if (args == null || args.Count <= pos)
                return defaultValue;

            string arg = args[pos];

            T val;
            if (!Parse<T>(arg, out val))
                throw new ArgumentException(errorMsg);
            return val;
        }
        
        //PATTERN Фабричный метод (простейший вариант)
        static IFilter CreateFilter(string filterName, List<string> args)
        {
            //количество аргументов
            int argsCount = args == null ? 0 : args.Count;
            try
            {
                switch (filterName)
                {
                    case FilterNames.FILTER_BLUR:
                        {
                            int size = ParseArg<int>(args, 0, 3, "Invalid size");
                            return new ConvolutionalFilter(Kernels.GetUniformKernel(size, size));
                        }
                    case FilterNames.FILTER_GAUSSIAN_BLUR:
                        {
                            int size = ParseArg<int>(args, 0, 3, "Invalid size");
                            float sigma = ParseArg<float>(args, 1, 1, "Invalid StdDev");

                            return new ConvolutionalFilter(Kernels.GetGaussianKernel(size, sigma));
                        }
                    case FilterNames.FILTER_UNIFORM_NOISE:
                        {
                            int amplitude = ParseArg<int>(args, 0, 3, "Invalid amplitude");
                            return new UniformNoise(amplitude);
                        }
                    case FilterNames.FILTER_GAUSSIAN_NOISE:
                        {
                            float sigma = ParseArg<float>(args, 0, 0.15f, "Invalid StdDev");
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
                    case FilterNames.FILTER_SHARR:
                        {
                            return new EdgeDetection(EdgeDetection.EdgeDetectionAlgorithm.Sharr);
                        }
                    case FilterNames.FILTER_SOBEL:
                        {
                            return new EdgeDetection(EdgeDetection.EdgeDetectionAlgorithm.Sobel);
                        }
                    case FilterNames.FILTER_CONTRAST:
                        {
                            float maxValue = ParseArg<float>(args, 0, 255, "Invalid max value");
                            float minValue = ParseArg<float>(args, 1, 0, "Invalid min value");

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
                    case FilterNames.FILTER_BINARIZATION:
                        {
                            float edge = ParseArg<int>(args, 0, 128, "Invalid threshold value");
                            return new BinarizationFilter(edge);
                        }
                    case FilterNames.FILTER_IMPULSE_NOISE:
                        {
                            float p = ParseArg<float>(args, 0, 0.1f, "Invalid probability value");
                            return new ImpulseNoise(p);
                        }
                    case FilterNames.FILTER_GRAYSCALE:
                        {
                            return new ColorSpaceConversion(ColorSpaceConversion.ConversionType.RGB2Gray);
                        }
                    case FilterNames.FILTER_LOW_PASS:
                        {
                            float s = ParseArg<float>(args, 0, 0.15f, "Invalid parameter");
                            return new FrequencyFilter(FrequencyFilter.FilterMode.LowPass, s);
                        }
                    case FilterNames.FILTER_HIGH_PASS:
                        {
                            float s = ParseArg<float>(args, 0, 0.15f, "Invalid parameter");
                            return new FrequencyFilter(FrequencyFilter.FilterMode.HighPass, s);
                        }
                    default:
                        throw new NotSupportedException("Unsupported filter");
                }
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException(filterName + ": " + e.Message, e);
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
                //args = new[] { "-f", "LowPass","0.9", "-f", "Contrast",  @"D:\WORK\Data\Tmp\IN\PVD.bmp", @"D:\WORK\Data\Tmp\OUT\PVD.png" };
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
                Console.WriteLine("Unexpected error: " + e.Message);
                return;
            }
        }
    }
}
