namespace StreamWatchService
{
    using System.ServiceProcess;
    using System.Threading.Tasks;

    using Nito.AsyncEx;

    using NLog;

    using StreamWatchService.ServiceHelpers;

    [WindowsService("StreamWatch", CanPauseAndContinue = false, Description = ServiceDescription, DisplayName = ServiceDisplayName,
        StartMode = ServiceStartMode.Automatic)]
    public class StreamWatchWindowsService : IWindowsService
    {
        private const string ServiceDescription = "Watches online streams for content and records them.";

        private const string ServiceDisplayName = "Stream Watch Service";

        private readonly Logger logger;

        private readonly LiveStreamerResource liveStreamerResource;

        public StreamWatchWindowsService(LiveStreamerResource liveStreamerResource, Logger logger)
        {
            this.liveStreamerResource = liveStreamerResource;
            this.logger = logger;
        }

        protected bool Disposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void OnContinue()
        {
            this.logger.Info("Service continued.");
        }

        public void OnCustomCommand(int command)
        {
        }

        public void OnPause()
        {
            this.logger.Info("Service paused.");
        }

        public void OnShutdown()
        {
            this.logger.Info("Service shutdown.");
        }

        public void OnStart(string[] arguments)
        {
            this.logger.Info("Service started.");

            AsyncContext.Run(() => this.RunServiceAsync());
        }

        public void OnStop()
        {
            this.logger.Info("Service stopped.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                this.Disposed = true;
            }
        }

        private async Task RunServiceAsync()
        {
            var state = await this.liveStreamerResource.GetLiveStreamerStateAsync();

            if (state != LiveStreamerInstallState.Installed)
            {
                if (await this.liveStreamerResource.InstallAsync() != LiveStreamerInstallState.Installed)
                {
                    this.logger.Trace("Failed to find or extract necessary files for livestreamer.");
                    this.OnStop();
                }
            }
        }
    }
}