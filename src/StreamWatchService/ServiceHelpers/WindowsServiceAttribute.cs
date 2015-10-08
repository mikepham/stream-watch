namespace StreamWatchService.ServiceHelpers
{
    using System;
    using System.ServiceProcess;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class WindowsServiceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the windows service.</param>
        public WindowsServiceAttribute(string name)
        {
            // Set name and default description and display name to name.
            this.Description = name;
            this.DisplayName = name;
            this.Name = name;

            // Default all other attributes.
            this.Account = ServiceAccount.LocalService;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
            this.EventLogSource = null;
            this.Password = null;
            this.StartMode = ServiceStartMode.Manual;
            this.UserName = null;
        }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the displayable name that shows in service manager (default: Name).
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the textural description of the service name (default: Name).
        /// </summary>
        /// qqq
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ServiceAccount" /> that is used to run the service.
        /// </summary>
        public ServiceAccount Account { get; set; }

        /// <summary>
        /// Gets or sets the user to run the service under (default: null).  A null or empty
        /// UserName field causes the service to run as ServiceAccount.LocalService.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password to run the service under (default: null).  Ignored
        /// if the UserName is empty or null, this property is ignored.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the event log source to set the service's EventLog to.  If this is
        /// empty or null (the default) no event log source is set.  If set, will auto-log
        /// start and stop events.
        /// </summary>
        public string EventLogSource { get; set; }

        /// <summary>
        /// Gets or sets the method to start the service when the machine reboots (default: Manual).
        /// </summary>
        public ServiceStartMode StartMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow pause and continue (default: true).
        /// </summary>
        public bool CanPauseAndContinue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow shutdown (default: true).
        /// </summary>
        public bool CanShutdown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow the ability to stop (default: true).
        /// </summary>
        public bool CanStop { get; set; }
    }
}