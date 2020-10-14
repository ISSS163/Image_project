using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Library
{
    public abstract class ColorSpace
    {
        public abstract (float, float, float) FromRGB((float, float, float) pixel);
        public abstract (float, float, float) ToRGB((float, float, float) pixel);
    }

    /// <summary>
    /// Цветовое пространство HSV (Hue-Saturation-Value / Цвет-насыщенность-яркость)
    /// </summary>
    public class HSV : ColorSpace
    {
        public override (float, float, float) FromRGB((float, float, float) pixel)
        {
            (float r, float g, float b) = pixel;
            r /= 255;
            g /= 255;
            b /= 255;
            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            float h, s, v;
            if(max == min)
                h = 0;
            else if (max == r) 
            {
                h = 60 * (g - b) / (max - min);
                if (h < 0)
                    h += 360;
                if (Math.Abs(h) > 360)
                    h -= Math.Sign(h) * 360;
            }
            else if (max == g) 
            {
                h= 60 * (b - r) / (max - min) + 120;
            }
            else 
            {
                h = 60 * (r - g) / (max - min) +240;
            }
            s = 1 - min / max;
            v = max;

            h *= 255.0f / 360;
            s *= 255;
            v *= 255;
            return (h, s, v);
        }

        public override (float, float, float) ToRGB((float, float, float) pixel)
        {
            (float h, float s, float v) = pixel;
            float r, g, b;
            h*= 360 / 255.0f;
            s *= 100/255.0f;
            v *= 100/ 255.0f;

            int h_i = (int)Math.Floor(h / 60) % 6;
            float v_min = (100 - s) * v/100;
            float a = (v - v_min) * ((int)h % 60) / 60.0f;
            float v_inc = v_min + a;
            float v_dec = v - a;

            switch (h_i)
            {
                case 0:
                    r = v; g = v_inc; b = v_min;
                    break;
                case 1:
                    r = v_dec; g = v; b = v_min;
                    break;
                case 2:
                    r = v_min; g = v; b = v_inc;
                    break;
                case 3:
                    r = v_min; g = v_dec; b = v;
                    break;
                case 4:
                    r = v_inc; g = v_min; b = v;
                    break;
                case 5:
                    r = v; g = v_min; b = v_dec;
                    break;
                default:
                    throw new Exception("Should never be here");
            }
            return (r*255.0f/100, g * 255.0f / 100, b * 255.0f / 100);
        }
    }

    /// <summary>
    /// Цветовое пространство YCbCr (Яркость + 2 цветоразностные компоненты)
    /// </summary>
    /// <remarks>https://ru.wikipedia.org/wiki/YCbCr#Преобразования_JPEG</remarks>
    public class YCbCr : ColorSpace
    {
        public override (float, float, float) FromRGB((float, float, float) pixel)
        {
            float y, cb, cr, r, g, b;
            (r, g, b) = pixel;
            y = 0 + 0.299f * r + 0.587f * g + 0.117f * b;
            cb = 128 - 0.168736f * r - 0.331264f * g + 0.5f * b;
            cr = 128 + 0.5f * r - 0.418688f * g - 0.081312f * b;

            return (y, cb, cr);
        }

        public override (float, float, float) ToRGB((float, float, float) pixel)
        {
            float y, cb, cr, r, g, b;
            (y, cb, cr) = pixel;

            r = y + 1.402f * (cr - 128);
            g = y - 0.34414f * (cb - 128) - 0.71414f * (cr - 128);
            b = y + 1.772f * (cb - 128);

            return (r, g, b);
        }
    }

    /// <summary>
    /// Оттенки серого
    /// </summary>
    /// <remarks>
    /// https://en.wikipedia.org/wiki/Grayscale#Luma_coding_in_video_systems
    /// </remarks>
    public class GrayScale : ColorSpace
    {
        public override (float, float, float) FromRGB((float, float, float) pixel)
        {
            float r, g, b, gray;
            (r, g, b) = pixel;
            gray = 0.299f * r + 0.587f * g + 0.114f * b;
            return (gray, gray, gray);
        }

        public override (float, float, float) ToRGB((float, float, float) pixel)
        {
            return pixel; //нельзя восстановить RGB из GrayScale
        }
    }

}
