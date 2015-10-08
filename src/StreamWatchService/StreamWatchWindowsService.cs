namespace StreamWatchService
{
    using System;
    using System.ServiceProcess;
    using System.Threading.Tasks;

    using Microsoft.Owin.Hosting;

    using Nito.AsyncEx;

    using NLog;

    using PowerArgs;

    using StreamWatchService.LiveStreamer;
    using StreamWatchService.ServiceHelpers;

    [WindowsService("StreamWatch", CanPauseAndContinue = false, Description = ServiceDescription, DisplayName = ServiceDisplayName,
        StartMode = ServiceStartMode.Automatic)]
    public class StreamWatchWindowsService : IWindowsService
    {
        private const string ServiceDescription = "Watches online streams for content and records them.";

        private const string ServiceDisplayName = "Stream Watch Service";

        private readonly Logger logger;

        private readonly LiveStreamerResource liveStreamerResource;

        private IDisposable server;

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
            var options = Args.Parse<StreamWatchOptions>(arguments);

            this.logger.Info("Service started.");
            this.ConfigureApi(options);

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

                if (this.server != null)
                {
                    this.server.Dispose();
                    this.server = null;
                }
            }
        }

        private void ConfigureApi(StreamWatchOptions options)
        {
            this.server = WebApp.Start<Startup>(this.CreateStartOptions(options));
        }

        private StartOptions CreateStartOptions(StreamWatchOptions options)
        {
            var url = new UriBuilder(Uri.UriSchemeHttp, "localhost", options.Port);
            return new StartOptions(url.Uri.AbsoluteUri);
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