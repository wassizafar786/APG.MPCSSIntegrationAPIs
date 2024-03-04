using Apache.NMS.ActiveMQ.Commands;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.External.Mpcss.Helper
{
    public class ManualResetProvider : IManualResetProvider
    {

        private ConcurrentDictionary<string, ManualResetEvent> resetEventDictionary = new();

        public void AddManualReset(string manualResetEventKey, ManualResetEvent manualResetEvent)
        {
            resetEventDictionary.TryAdd(manualResetEventKey, manualResetEvent);

        }

        public void FinishManualReset(string manualResetEventKey)
        {  

            var manualResetEventResult = resetEventDictionary.TryGetValue(manualResetEventKey,out ManualResetEvent manualResetEvent);

            if (manualResetEventResult)
            {
                manualResetEvent.Set();
                resetEventDictionary.TryRemove(manualResetEventKey, out _);
            }
        }
    }
}