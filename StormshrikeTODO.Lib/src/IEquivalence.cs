using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Model
{
    public interface IEquivalence<T>
    {
        bool IsEquivalentTo(T other);
    }
}
