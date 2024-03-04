using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.External.Interfaces
{
    public interface IBaseHostFactory<T>
    {
        T CreateHost(params object[] args);
    }
}
