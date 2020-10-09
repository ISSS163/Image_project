using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using SkiaSharp;

namespace Library
{
    public enum PaddingType
    {
        /// <summary>
        /// Новые пиксели заполняются 0
        /// </summary>
        ZERO,
        /// <summary>
        /// Новые пиксели заполняются значениями ближайшего пикселя исходного изображения
        /// </summary>
        EDGE,
        /// <summary>
        /// Значения новых пикселей копируются из пикселей с другой стороны изображения (изображение как бы "сворачивается (wrap) в трубочку", так что один край совмещается с другим  )
        /// </summary>
        WRAP,
        /// <summary>
        /// Значения новых пикселей берутся зеркальным отражением относительно границы 
        /// </summary>
        REFLECT
    }

    public class Image
    {
        /// <summary>
        /// Ширина изображения
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Высота изображения
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Число каналов
        /// </summary>
        public int Channels { get { return 3; } }

        private float[,,] _data;
        public float[,,] Data { get { return _data; } }

        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public float this[int i, int j, int k]
        {
            get { return _data[i, j, k]; }
            set { _data[i, j, k] = value; }
        }

        /// <summary>
        /// Констркутор, создающий изображение заданных размеров с пикселями, заполненными 0
        /// </summary>
        /// <param name="w">Ширина</param>
        /// <param name="h">Высота</param>
        public Image( int h, int w)
        {
            if (w < 0 || h < 0)
                throw new ArgumentException(string.Format("Invalid size ({0},{1}) was specified", w, h));
            Width = w;
            Height = h;
            _data = new float[h, w, 3];
        }

        /// <summary>
        /// Открыть файл с изображением
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Image Open(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("File does not exists");
            }
            using(SKBitmap image = SKBitmap.Decode(path))
            {
                if (image == null)
                {
                    throw new ArgumentException("Failed to open image");
                }
                var result = new Image(image.Height, image.Width);
                for (int i = 0; i < image.Height; i++)
                    for (int j = 0; j < image.Width; j++)
                    {
                        var pixel = image.GetPixel(j, i);
                        result[i, j, 0] = pixel.Red;
                        result[i, j, 1] = pixel.Green;
                        result[i, j, 2] = pixel.Blue;
                    }
                return result;
            }
        }

        #region a_n_s_w_e_r_1
        //Contrast
        #endregion

        /// <summary>
        /// Приведение byte к float c обрезкой диапазона до [0, 255]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        static byte Cast(float f)
        {
            return (byte)Math.Clamp(f, 0, 255);
        }

        /// <summary>
        /// Словарь форматов, в которые возможно сохранение
        /// </summary>
        private static readonly Dictionary<string, SKEncodedImageFormat> _allowedFormats = new Dictionary<string, SKEncodedImageFormat> {
            //{ ".bmp", SKEncodedImageFormat.Bmp },
            { ".png",SKEncodedImageFormat.Png },
            { ".jpg",SKEncodedImageFormat.Jpeg },
            { ".jpeg", SKEncodedImageFormat.Jpeg } 
        };

        public static bool CanSaveImage(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return _allowedFormats.ContainsKey(ext);
        }

        static SKEncodedImageFormat GetFormat(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return _allowedFormats[ext];
        }

        /// <summary>
        /// Сохранение изображения в файл
        /// </summary>
        /// <param name="path">Путь, по которому изображение будет сохранено. Если папки в этом пути не существуют, они будут созданы</param>
        public void Save(string path)
        {
            if (!CanSaveImage(path))
                throw new ArgumentException("Can not save image using specified image format");
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            
            using(var outImage = new SKBitmap(this.Width, this.Height, SKColorType.Rgb888x, SKAlphaType.Opaque))
            {
                for (int i = 0; i < this.Height; i++)
                    for (int j = 0; j < this.Width; j++)
                    {
                        SKColor color = new SKColor(Cast(this[i, j, 0]), Cast(this[i, j, 1]), Cast(this[i, j, 2]));
                        outImage.SetPixel(j, i, color);
                    }

                using (SKImage img = SKImage.FromBitmap(outImage))
                using (var data = img.Encode(GetFormat(path), 100))
                using (var stream = File.Create(path))
                    data.SaveTo(stream);
            }
        }

        private static float GetPadValue(Image baseImage, int vPos, int hPos, (int, int) vPadSize, (int, int) hPadSize,int channel, PaddingType t)
        {
            if (t == PaddingType.ZERO)
                return 0.0f;
            if(t == PaddingType.EDGE)
            {
                vPos = vPos < vPadSize.Item1 ? 0 :  Math.Min(vPos - vPadSize.Item1, baseImage.Height -1);
                hPos =  hPos < hPadSize.Item1 ? 0 : Math.Min(hPos - hPadSize.Item1, baseImage.Width - 1);
                return baseImage[vPos, hPos, channel];
            }
            if (t == PaddingType.REFLECT)
            {
                vPos = vPos < vPadSize.Item1 ? vPadSize.Item1 - vPos : (2*baseImage.Height - vPos + vPadSize.Item1-2);
                hPos = hPos < hPadSize.Item1 ? hPadSize.Item1 - hPos : (2*baseImage.Width - hPos + vPadSize.Item1-2);

                vPos %= baseImage.Height;
                hPos %= baseImage.Width;
                if (vPos < 0) vPos += baseImage.Height;
                if (hPos < 0) hPos += baseImage.Width;
                return baseImage[vPos, hPos, channel] ;
            }
            if (t == PaddingType.WRAP)
                throw new NotImplementedException();
            throw new Exception("Should newer be here");
        }


        public static Image Pad(Image image, ValueTuple<int, int> vPad, ValueTuple<int, int> hPad, PaddingType t = PaddingType.ZERO)
        {
           Common.ThrowIfNull(image, nameof(image));

            if (vPad.Item1 < 0 || vPad.Item2 < 0)
                throw new ArgumentException(nameof(vPad));
            if(hPad.Item1 < 0 || hPad.Item2 < 0)
                throw new ArgumentException(nameof(hPad));


            (int hPadLeft, int hPadRight) = hPad;
            (int vPadDown, int vPadUp) = vPad;

            int newWidth = image.Width + hPadLeft + hPadRight;
            int newHeight = image.Height + vPadUp + vPadDown;


            var result = new Image(newHeight, newWidth);

            for (int i = 0; i < result.Height; i++)
                for (int j = 0; j < result.Width; j++)
                {
                    bool needPad = (i < vPadDown) || (i >= vPadDown + image.Height) || (j < hPadLeft) || (j >= hPadLeft + image.Width);
                    for (int k = 0; k < result.Channels; k++)
                        result[i, j, k] = needPad ? GetPadValue(image, i, j, vPad, hPad, k, t): image[i-vPadDown, j-hPadLeft, k];
                }

            return result;
        }
    }
}
