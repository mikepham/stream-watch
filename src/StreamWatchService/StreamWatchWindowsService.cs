namespace StreamWatchService
{
    using StreamWatchService.ServiceHelpers;

    [WindowsService("StreamWatch")]
    public class StreamWatchWindowsService : IWindowsService
    {
        protected bool Disposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void OnContinue()
        {
        }

        public void OnCustomCommand(int command)
        {
        }

        public void OnPause()
        {
        }

        public void OnShutdown()
        {
        }

        public void OnStart(string[] arguments)
        {
        }

        public void OnStop()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                this.Disposed = true;
            }
        }
    }
}