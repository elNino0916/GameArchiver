using System;
using System.Globalization;
using System.Threading;

namespace GameArchiver
{
    /// <summary>
    /// Console UI helpers for colored output, spinners, and progress bars.
    /// </summary>
    public static class ConsoleUI
    {
        private const string Banner =
@"   ____                         _             _     _                
  / ___| __ _ _ __ ___   ___   / \   _ __ ___| |__ (_)_   _____ _ __ 
 | |  _ / _` | '_ ` _ \ / _ \ / _ \ | '__/ __| '_ \| \ \ / / _ \ '__|
 | |_| | (_| | | | | | |  __// ___ \| | | (__| | | | |\ V /  __/ |   
  \____|\__,_|_| |_| |_|\___/_/   \_\_|  \___|_| |_|_| \_/ \___|_|   
                                                                     ";

        public static void SetConsoleTheme()
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch { /* ignore */ }
        }

        public static void WriteBanner()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Banner);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteSection(string title)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"== {title} ==");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteLineGold(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteLineGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteLineRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static bool Confirm(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.Gray;

            while (true)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.Y) { Console.WriteLine("Y"); return true; }
                if (key == ConsoleKey.N) { Console.WriteLine("N"); return false; }
            }
        }

        public static void RunWithSpinner(string message, Action action)
        {
            using var done = new ManualResetEventSlim(false);
            Exception? ex = null;

            var worker = new Thread(() =>
            {
                try { action(); }
                catch (Exception e) { ex = e; }
                finally { done.Set(); }
            })
            { IsBackground = true };

            worker.Start();

            var spinner = new[] { '?', '?', '?', '?', '?', '?', '?', '?' };
            int i = 0;

            while (!done.IsSet)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"\r{spinner[i++ % spinner.Length]} ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(message + "   ");
                Thread.Sleep(80);
            }

            Console.Write("\r   " + new string(' ', message.Length + 6) + "\r");

            if (ex != null) throw ex;
        }

        public static void DrawProgressBar(int percent)
        {
            const int width = 40;
            int filled = (percent * width) / 100;
            string bar = new string('?', filled) + new string('?', width - filled);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"\r[{bar}] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{percent.ToString(CultureInfo.InvariantCulture),3}%");
        }
    }
}
