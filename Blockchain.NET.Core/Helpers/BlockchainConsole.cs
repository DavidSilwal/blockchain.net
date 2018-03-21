using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Core.Helpers
{
    public enum ConsoleEventType
    {
        Elapsed = ConsoleColor.White,
        MINEDBLOCK = ConsoleColor.Green,
        WriteLive = ConsoleColor.Yellow
    }

    public static class BlockchainConsole
    {
        public static void WriteLine(object value, ConsoleEventType color)
        {
            Console.ForegroundColor = (ConsoleColor)color;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        public static void WriteLive(object value)
        {
            Console.ForegroundColor = (ConsoleColor)ConsoleEventType.WriteLive;
            Console.Write($"\r{value}");
            Console.ResetColor();
        }
    }
}
