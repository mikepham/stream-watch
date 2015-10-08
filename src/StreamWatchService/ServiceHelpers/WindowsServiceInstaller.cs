namespace StreamWatchService.ServiceHelpers
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;

    /// <summary>
    /// A generic windows service installer
    /// </summary>
    [RunInstaller(true)]
    public partial class WindowsServiceInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceInstaller" /> class.
        /// </summary>
        public WindowsServiceInstaller()
            : this(typeof(StreamWatchWindowsService))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceInstaller" /> class.
        /// </summary>
        /// <param name="windowsServiceType">The type of the windows service to install.</param>
        public WindowsServiceInstaller(Type windowsServiceType)
        {
            if (!windowsServiceType.GetInterfaces().Contains(typeof(IWindowsService)))
            {
                throw new ArgumentException("Type to install must implement IWindowsService.", "windowsServiceType");
            }

            var attribute = windowsServiceType.GetTypeInfo().GetCustomAttribute<WindowsServiceAttribute>();

            if (attribute == null)
            {
                throw new ArgumentException("Type to install must be marked with a WindowsServiceAttribute.", "windowsServiceType");
            }

            this.Configuration = attribute;
        }

        /// <summary>
        /// Gets or sets the type of the windows service to install.
        /// </summary>
        public WindowsServiceAttribute Configuration { get; set; }

        /// <summary>
        /// Performs a transacted installation at run-time of the AutoCounterInstaller and any other listed installers.
        /// </summary>
        /// <typeparam name="T">The IWindowsService implementer to install.</typeparam>
        public static void RuntimeInstall<T>() where T : IWindowsService
        {
            var path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new WindowsServiceInstaller(typeof(T)));
                ti.Context = new InstallContext(null, new[] { path });
                ti.Install(new Hashtable());
            }
        }

        /// <summary>
        /// Performs a transacted un-installation at run-time of the AutoCounterInstaller and any other listed installers.
        /// </summary>
        /// <param name="otherInstallers">The other installers to include in the transaction</param>
        /// <typeparam name="T">The IWindowsService implementer to install.</typeparam>
        public static void RuntimeUnInstall<T>(params Installer[] otherInstallers) where T : IWindowsService
        {
            var path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new WindowsServiceInstaller(typeof(T)));
                ti.Context = new InstallContext(null, new[] { path });
                ti.Uninstall(null);
            }
        }

        /// <summary>
        /// Installs the service.
        /// </summary>
        /// <param name="savedState">The saved state for the installation.</param>
        public override void Install(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Installing service {0}.", this.Configuration.Name);

            this.ConfigureInstallers();

            base.Install(savedState);

            if (!string.IsNullOrWhiteSpace(this.Configuration.EventLogSource))
            {
                if (!EventLog.SourceExists(this.Configuration.EventLogSource))
                {
                    EventLog.CreateEventSource(this.Configuration.EventLogSource, "Application");
                }
            }
        }

        /// <summary>
        /// Uninstalls the service.
        /// </summary>
        /// <param name="savedState">The saved state for the installation.</param>
        public override void Uninstall(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Un-Installing service {0}.", this.Configuration.Name);

            this.ConfigureInstallers();

            base.Uninstall(savedState);

            if (!string.IsNullOrWhiteSpace(this.Configuration.EventLogSource))
            {
                if (EventLog.SourceExists(this.Configuration.EventLogSource))
                {
                    EventLog.DeleteEventSource(this.Configuration.EventLogSource);
                }
            }
        }

        /// <summary>
        /// Rolls back to the state of the counter, and performs the normal rollback.
        /// </summary>
        /// <param name="savedState">The saved state for the installation.</param>
        public override void Rollback(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Rolling back service {0}.", this.Configuration.Name);

            this.ConfigureInstallers();

            base.Rollback(savedState);
        }

        /// <summary>
        /// Method to configure the installers
        /// </summary>
        private void ConfigureInstallers()
        {
            this.Installers.Add(this.ConfigureProcessInstaller());
            this.Installers.Add(this.ConfigureServiceInstaller());
        }

        /// <summary>
        /// Helper method to configure a process installer for this windows service
        /// </summary>
        /// <returns>Process installer for this service</returns>
        private ServiceProcessInstaller ConfigureProcessInstaller()
        {
            var result = new ServiceProcessInstaller();

            if (this.Configuration.Account == ServiceAccount.User && !string.IsNullOrEmpty(this.Configuration.UserName))
            {
                result.Account = ServiceAccount.User;
                result.Username = this.Configuration.UserName;
                result.Password = this.Configuration.Password;
            }
            else
            {
                result.Account = this.Configuration.Account;
                result.Username = null;
                result.Password = null;
            }

            return result;
        }

        /// <summary>
        /// Helper method to configure a service installer for this windows service
        /// </summary>
        /// <returns>Process installer for this service</returns>
        private ServiceInstaller ConfigureServiceInstaller()
        {
            // create and config a service installer
            var result = new ServiceInstaller
                             {
                                 ServiceName = this.Configuration.Name,
                                 DisplayName = this.Configuration.DisplayName,
                                 Description = this.Configuration.Description,
                                 StartType = this.Configuration.StartMode,
                             };

            return result;
        }
    }
}