namespace StreamWatchService
{
    using System;
    using System.ServiceProcess;

    using NLog;

    using PowerArgs;

    using SimpleInjector;

    using StreamWatchService.Api.Controllers;
    using StreamWatchService.LiveStreamer;
    using StreamWatchService.ServiceHelpers;

    internal static class Program
    {
        private static readonly Lazy<Container> DefaultContainer = new Lazy<Container>(CreateDefaultContainer);

        internal static Container Container => DefaultContainer.Value;

        static Program()
        {
            SetupDependencies();
        }

        private static void Main(string[] arguments)
        {
            var options = Args.Parse<StreamWatchOptions>(arguments);

            if (IsSetupRequested(options))
            {
                Environment.Exit(0);
            }

            using (var application = Container.GetInstance<IWindowsService>())
            {
                if (Environment.UserInteractive || options.RunAsConsole)
                {
                    ConsoleHarness.Run(arguments, application);
                }
                else
                {
                    using (var harness = new WindowsServiceHarness(application))
                    {
                        ServiceBase.Run(harness);
                    }
                }
            }
        }

        private static Container CreateDefaultContainer()
        {
            return new Container();
        }

        private static bool IsSetupRequested(StreamWatchOptions options)
        {
            if (options.Install)
            {
                WindowsServiceInstaller.RuntimeInstall<StreamWatchWindowsService>();
            }
            else if (options.Uninstall)
            {
                WindowsServiceInstaller.RuntimeUnInstall<StreamWatchWindowsService>();
            }
            else
            {
                return false;
            }

            return true;
        }

        private static void SetupDependencies()
        {
            Container.Register<IWindowsService, StreamWatchWindowsService>(Lifestyle.Singleton);
            Container.Register<LiveStreamerResource>(Lifestyle.Singleton);
            Container.Register(() => LogManager.GetLogger("StreamWatchService"), Lifestyle.Singleton);
            Container.Register<LiveStreamerMonitor>();
            Container.Register<UserController>();
        }
    }
}