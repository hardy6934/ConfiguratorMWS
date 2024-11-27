

namespace ConfiguratorMWS.Data.Abstract
{
    public interface ITimerRepository
    {
        public void TimerWork(EventHandler timerCallback);
        public void ChangeTimerWorkInterval(int interval);
    }
}
