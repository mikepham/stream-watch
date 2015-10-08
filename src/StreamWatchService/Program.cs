namespace StreamWatchService
{
    using System;
    using System.ServiceProcess;

    using PowerArgs;

    using StreamWatchService.ServiceHelpers;

    internal static class Program
    {
        private static void Main(string[] arguments)
        {
            var options = Args.Parse<StreamWatchOptions>(arguments);

            if (IsSetupRequested(options))
            {
                Environment.Exit(0);
            }

            using (var application = new StreamWatchWindowsService())
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
    }
}