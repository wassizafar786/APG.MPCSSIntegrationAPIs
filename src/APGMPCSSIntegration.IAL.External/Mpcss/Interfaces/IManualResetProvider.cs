using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.External.Mpcss.Interfaces
{
    public interface IManualResetProvider
    {
        void AddManualReset(string manualResetEventKey, ManualResetEvent manualResetEvent);
        void FinishManualReset(string manualResetEventKey);
    }
}
