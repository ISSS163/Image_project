using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library
{
     /// <summary>
     /// Преобразование цветовых пространств
     /// </summary>
    public class ColorSpaceConversion: PixelwiseFilter
    {
        public ColorSpace ColorSpace { get;  }
        public bool IsForwardConversion { get; }

        public enum ConversionType
        {
            RGB2HSV = 0,
            HSV2RGB = 1,
            RGB2YCbCr = 2,
            YCbCr2RGB = 3,
            RGB2Gray = 4,
            Gray2RGB = 5,
        }

        public ColorSpaceConversion(ConversionType c)
        {
            //используем то, что у всех членов перечисления ConversionType указаны значения
            int type = (int)c / 2;
            int direction = (int)c % 2;
            ColorSpace = type == 0 ? new HSV() as ColorSpace: new YCbCr() as ColorSpace;
            IsForwardConversion = direction == 0;
        }

        protected override void ProcessPixel(Image image, int vPos, int hPos)
        {
            var pixel = (image[vPos, hPos, 0], image[vPos, hPos, 1], image[vPos, hPos, 2]);
            pixel = IsForwardConversion ? ColorSpace.FromRGB(pixel) : ColorSpace.ToRGB(pixel);
            (image[vPos, hPos, 0], image[vPos, hPos, 1], image[vPos, hPos, 2]) = pixel;
        }        
    }
}
