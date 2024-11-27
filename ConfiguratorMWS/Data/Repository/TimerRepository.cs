

using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using System.IO.Ports;
using System.Windows.Threading;

namespace ConfiguratorMWS.Data.Repository
{
    public class TimerRepository: ITimerRepository
    {
        private DispatcherTimer timer; 

        public TimerRepository()
        { 
            timer = new DispatcherTimer();
        }

        public void TimerWork(EventHandler timerCallback)
        {
            //таймер
            //установка начальных значений работы таймера
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += timerCallback;
            timer.Start();
        }
        public void ChangeTimerWorkInterval(int interval)
        {
            if (interval == 0)
            {
                timer.Stop();
            }
            else { 
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(interval);
                timer.Start();
            }

        }

    }
}
