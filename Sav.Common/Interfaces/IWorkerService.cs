namespace Sav.Common.Interfaces
{
    public interface IWorkerService
    {
        public bool IsStarted { get; }

        public bool IsRunning { get; }

        public Task StartAsync(CancellationToken token);
    }
}
