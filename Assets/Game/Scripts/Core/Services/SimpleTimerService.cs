using Game.Scripts.Core.Interfaces;

namespace Game.Scripts.Core.Services
{
    public class SimpleTimerService : ITimerService
    {
        public bool IsRunning { get; private set; }
        public float Elapsed { get; private set; }

        public void Start() => IsRunning = true;
        public void Stop()  => IsRunning = false;

        public void Reset()
        {
            IsRunning = false;
            Elapsed = 0f;
        }

        public void Tick(float deltaSeconds)
        {
            if (!IsRunning) return;
            Elapsed += deltaSeconds;
        }
    }
}