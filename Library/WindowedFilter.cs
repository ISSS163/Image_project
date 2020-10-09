using System;
using System.Collections.Generic;
using System.Text;


namespace Library
{
    /// <summary>
    /// Оконный фильтр
    /// </summary>
    public abstract class WindowedFilter : IFilter
    {
        /// <summary>
        /// Высота окна
        /// </summary>
        public int WindowHeight { get; }
        /// <summary>
        /// Ширина окна
        /// </summary>
        public int WindowWidth { get; }

        public WindowedFilter(int windowHeight, int windowWidth)
        {
            if (windowWidth < 1 || windowHeight < 1)
                throw new ArgumentException("Window size must be > 0");
            if (windowWidth % 2 == 0 || windowHeight % 2 == 0)
                throw new ArgumentException("Window size must be odd in both dimensions");
            WindowHeight = windowWidth;
            WindowWidth = windowWidth;
        }

        protected abstract void ProcessWindow(Image inImage, Image outImage, int vPos, int hPos);

        public void Apply(Image image)
        {
            Common.ThrowIfNull(image, nameof(image));

            //для того, чтобы нормально обработать пиксели на границах - дополним изображение
            var imagePadded = Image.Pad(image, (WindowHeight / 2, WindowHeight / 2), (WindowHeight / 2, WindowHeight / 2), PaddingType.EDGE);
            
            for(int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                {
                    ProcessWindow(imagePadded, image,i, j);
                }
        }
    }

}
