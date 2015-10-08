namespace StreamWatchService
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    using NLog;

    public class LiveStreamerResource
    {
        private const string LiveStreamerResourceExe = "livestreamer.exe";

        private const string LiveStreamerResourceName = "StreamWatchService.Resources.livestreamer-v1.12.2-win32.zip";

        private const string LiveStreamerResourceVersion = "livestreamer-v1.12.2";

        private readonly Logger logger;

        public LiveStreamerResource(Logger logger)
        {
            this.logger = logger;
        }

        public Task<LiveStreamerInstallState> GetLiveStreamerStateAsync()
        {
            var path = GetLiveStreamerInstallationPath();
            var filename = Path.Combine(path, LiveStreamerResourceExe);

            if (File.Exists(filename))
            {
                return Task.FromResult(LiveStreamerInstallState.Installed);
            }

            return Task.FromResult(LiveStreamerInstallState.Unknown);
        }

        public Task<LiveStreamerInstallState> InstallAsync()
        {
            var path = GetAppDataPath();

            try
            {
                using (var stream = GetResourceStream())
                using (var archive = new ZipArchive(stream))
                {
                    archive.ExtractToDirectory(path);
                }

                return Task.FromResult(LiveStreamerInstallState.Installed);
            }
            catch (Exception ex)
            {
                this.logger.Log(LogLevel.Fatal, ex, $"Failed to extract files to \"{path}\".");
                return Task.FromResult(LiveStreamerInstallState.Failed);
            }
        }

        private string GetAppDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), typeof(LiveStreamerResource).Assembly.GetName().Name);
        }

        private string GetLiveStreamerInstallationPath()
        {
            var directory = this.GetAppDataPath();
            var path = Path.Combine(directory, LiveStreamerResourceVersion);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                this.logger.Trace($"Created livestreamer directory at \"{path}\".");
            }

            return path;
        }

        private Stream GetResourceStream()
        {
            var assembly = this.GetType().Assembly;

            return assembly.GetManifestResourceStream(LiveStreamerResourceName);
        }
    }
}