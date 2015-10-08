namespace StreamWatchService.ServiceHelpers
{
    using System;
    using System.Diagnostics;

    public static class ConsoleHarness
    {
        /// <summary>
        /// Initializes static members of the <see cref="ConsoleHarness" /> class.
        /// </summary>
        static ConsoleHarness()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        /// <summary>
        /// Runs the service that implements <see cref="IWindowsService" />.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="service">The service.</param>
        public static void Run(string[] args, IWindowsService service)
        {
            var serviceName = service.GetType().Name;
            WriteToConsole(ConsoleColor.Green, serviceName);
            WriteToConsole(ConsoleColor.Yellow, "Enter either [Q]uit, [P]ause, [R]esume : ");

            var running = true;

            service.OnStart(args);

            while (running)
            {
                running = ReadConsoleInput(service, Console.ReadKey(true).Key.ToString());
            }

            service.OnStop();
            service.OnShutdown();
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public static void Pause()
        {
            WriteToConsole(ConsoleColor.Green, "Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Helper method that specifies a color to be used when writing to the <see cref="Console" />.
        /// </summary>
        /// <param name="foregroundColor">The foreground color.</param>
        /// <param name="format">The string format.</param>
        /// <param name="formatArguments">The format arguments.</param>
        public static void WriteToConsole(ConsoleColor foregroundColor, string format, params object[] formatArguments)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(format, formatArguments);
            Console.Out.Flush();

            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Reads the console input.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="line">The line.</param>
        /// <returns><c>true</c> if can continue, <c>false</c> otherwise.</returns>
        private static bool ReadConsoleInput(IWindowsService service, string line)
        {
            var canContinue = true;

            if (line != null)
            {
                switch (line.ToUpper())
                {
                    case "Q":
                        WriteToConsole(ConsoleColor.Red, "Quitting...");
                        canContinue = false;
                        break;

                    case "P":
                        WriteToConsole(ConsoleColor.DarkCyan, "Paused...");
                        service.OnPause();
                        break;

                    case "R":
                        WriteToConsole(ConsoleColor.Green, "Continuing...");
                        service.OnContinue();
                        break;

                    default:
                        WriteToConsole(ConsoleColor.Red, "Please select a a valid option.");
                        break;
                }
            }

            return canContinue;
        }
    }
}