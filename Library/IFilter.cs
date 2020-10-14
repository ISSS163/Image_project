using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    //PATTERN Strategy
    
    /// <summary>
    /// Интерфейс фильтра
    /// </summary>
    public interface IFilter
    {
        void Apply(Image image);
    }
}
