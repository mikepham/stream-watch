namespace StreamWatchService
{
    using PowerArgs;

    public class StreamWatchOptions
    {
        [ArgShortcut("i")]
        public bool Install { get; set; }

        [DefaultValue(17476)]
        [ArgShortcut("p")]
        public int Port { get; set; }

        [ArgShortcut("console")]
        [DefaultValue(false)]
        public bool RunAsConsole { get; set; }

        [ArgShortcut("u")]
        public bool Uninstall { get; set; }
    }
}