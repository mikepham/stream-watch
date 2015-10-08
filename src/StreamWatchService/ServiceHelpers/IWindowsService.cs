namespace StreamWatchService.ServiceHelpers
{
    using System;

    public interface IWindowsService : IDisposable
    {
        void OnStart(string[] arguments);

        void OnStop();

        void OnPause();

        void OnContinue();

        void OnShutdown();

        void OnCustomCommand(int command);
    }
}