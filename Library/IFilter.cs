using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    /// <summary>
    /// Интерфейс фильтра
    /// </summary>
    public interface IFilter
    {
        void Apply(Image image);
    }
}
