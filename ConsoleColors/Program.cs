using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;

namespace ConsoleColors
{
    class Program
    {

        private const int STD_INPUT_HANDLE = -10;

        private const int STD_OUTPUT_HANDLE = -11;

        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        const string PATH = @"colors.txt";
        const string ESC_TEMPLATE = "\u001b[{0}m";
        const string ESC_RESET = "\u001b[0m";
        static void Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                SetConsoleVirtualProcessing();

            var lines = File.ReadLines(PATH);

            int i = 0;
            foreach (var line in lines)
            {
                Console.SetCursorPosition(i * 41, Console.CursorTop);
                PrintColor(line);
                Console.Write("\t");
                i++;
                if (i > 2)
                {
                    i = 0;
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        static void PrintColor(string line)
        {
            string[] elements = line.Split('\t');
            string text = string.Format("{0}{1}{2}", string.Format(ESC_TEMPLATE, elements[0]), $"{elements[0].PadLeft(3)} {elements[1]}".PadRight(36, ' '), ESC_RESET);
            Console.Write(text);
        }

        static void SetConsoleVirtualProcessing(bool input = false, bool output = true)
        {
            if (input)
            {
                var iStdIn = GetStdHandle(STD_INPUT_HANDLE);

                if (!GetConsoleMode(iStdIn, out uint inConsoleMode))
                {
                    Console.WriteLine("failed to get input console mode");
                    Console.ReadKey();
                    return;
                }

                inConsoleMode |= ENABLE_VIRTUAL_TERMINAL_INPUT;

                if (!SetConsoleMode(iStdIn, inConsoleMode))
                {
                    Console.WriteLine($"failed to set input console mode, error code: {GetLastError()}");
                    Console.ReadKey();
                    return;
                }
            }
            
            if (output)
            {
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

                if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
                {
                    Console.WriteLine("failed to get output console mode");
                    Console.ReadKey();
                    return;
                }

                outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;

                if (!SetConsoleMode(iStdOut, outConsoleMode))
                {
                    Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                    Console.ReadKey();
                    return;
                }
            }
        }
    }
}

