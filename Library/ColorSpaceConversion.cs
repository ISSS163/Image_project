using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library
{
    //PATTERN Адаптер (или Фасад, зависит от того, как смотреть)

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
        }

        public ColorSpaceConversion(ConversionType c)
        {
            //используем то, что у всех членов перечисления ConversionType указаны значения
            int type = (int)c / 2;
            int direction = (int)c % 2;
            switch (type)
            {
                case 0:
                    ColorSpace = new HSV();
                    break;
                case 1:
                    ColorSpace = new YCbCr();
                    break;
                case 2:
                    ColorSpace = new GrayScale();
                    break;
                default:
                    throw new ArgumentException("Unknown color space");
            }

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
