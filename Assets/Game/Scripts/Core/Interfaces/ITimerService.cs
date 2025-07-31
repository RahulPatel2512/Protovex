namespace Game.Scripts.Core.Interfaces
{
    public interface ITimerService
    {
        bool IsRunning { get; }
        float Elapsed { get; }

        void Start();
        void Stop();
        void Reset();
        void Tick(float deltaSeconds);
    }
}