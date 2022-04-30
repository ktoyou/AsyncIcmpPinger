using Microsoft.Extensions.Logging;
using PingerInfo.Core;
using PingerInfo.Core.Abstractions;
using PingerInfo.Core.DB;
using System.Diagnostics;

namespace PingerInfo
{
    internal class Startup
    {
        static void Main(string[] args)
        {
            _dbConfiguration = DBConfiguration.LoadConfiguration("config.json");
            _pingers = new List<BasePinger>();
            _pingers.Add(
                new MySqlPinger(5000, 500, new DbApplicationContext(_dbConfiguration),
                new Logger<MySqlPinger>(LoggerFactory.Create(builder => { builder.AddConsole(); })))
            );

            _pingers.ForEach(async pinger => { await pinger.StartAsync(); });

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.C) break;
            }
        }

        private static List<BasePinger>? _pingers;

        private static DBConfiguration? _dbConfiguration;
    }
}
