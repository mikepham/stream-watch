namespace StreamWatchService.LiveStreamer
{
    public class LiveStreamerMonitor
    {
        private readonly LiveStreamerResource liveStreamerResource;

        public LiveStreamerMonitor(LiveStreamerResource liveStreamerResource)
        {
            this.liveStreamerResource = liveStreamerResource;
        }

        public LiveStreamerState State { get; private set; }
    }
}