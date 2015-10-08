namespace StreamWatchService.ServiceHelpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.ServiceProcess;

    /// <summary>
    /// A generic Windows Service that can handle any assembly that
    /// implements IWindowsService (including AbstractWindowsService).
    /// </summary>
    public partial class WindowsServiceHarness : ServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceHarness" /> class.
        /// </summary>
        /// <param name="service">Service implementation.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Reviewed.")]
        public WindowsServiceHarness(IWindowsService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service", "IWindowsService cannot be null in call to GenericWindowsService");
            }

            this.ConfigureServiceFromAttributes(service);
            this.Service = service;
        }

        /// <summary>
        /// Gets the class implementing the windows service
        /// </summary>
        public IWindowsService Service { get; private set; }

        protected override void OnContinue()
        {
            this.Service.OnContinue();
        }

        protected override void OnCustomCommand(int command)
        {
            this.Service.OnCustomCommand(command);
        }

        protected override void OnPause()
        {
            this.Service.OnPause();
        }

        protected override void OnShutdown()
        {
            this.Service.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            this.Service.OnStart(args);
        }

        protected override void OnStop()
        {
            this.Service.OnStop();
        }

        private void ConfigureServiceFromAttributes(IWindowsService service)
        {
            var attribute = service.GetType().GetTypeInfo().GetCustomAttribute<WindowsServiceAttribute>();

            if (attribute != null)
            {
                if (!string.IsNullOrWhiteSpace(attribute.EventLogSource))
                {
                    this.EventLog.Source = attribute.EventLogSource;
                }

                this.AutoLog = true;
                this.CanHandlePowerEvent = false;
                this.CanHandleSessionChangeEvent = false;
                this.CanPauseAndContinue = attribute.CanPauseAndContinue;
                this.CanShutdown = attribute.CanShutdown;
                this.CanStop = attribute.CanStop;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("IWindowsService implementer {0} must have a WindowsServiceAttribute.", service.GetType().FullName));
            }
        }
    }
}