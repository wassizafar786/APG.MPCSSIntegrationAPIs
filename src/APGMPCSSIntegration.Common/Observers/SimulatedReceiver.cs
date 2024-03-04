using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Common.Observers
{
    public interface IObserver
    {
        void SimulatedReceive(ISimulatedReceiver subject);
    }

    public interface ISimulatedReceiver
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        public void Trigger();

    }
    public class SimulatedReceiver : ISimulatedReceiver
    {
        public int State { get; set; } = -0;

        private List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            this._observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
        }

        public void Trigger()
        {
            Thread.Sleep(2000);
            this.Notify();
        }
        private void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.SimulatedReceive(this);
            }
        }
    }
}
