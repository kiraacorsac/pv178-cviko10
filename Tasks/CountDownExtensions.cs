using System.Threading;
using System.Threading.Tasks;
using Threads;

namespace Tasks
{
    public static class CountDownExtensions
    {
        public static void StartWithTask(this Countdown countdown)
        {
            Task.Run(() => {
                while (countdown.Enabled)
                {
                    Thread.Sleep(countdown.SecondsPerTick*1000);
                    countdown.Tick();
                }
            });
        }
    }
}
