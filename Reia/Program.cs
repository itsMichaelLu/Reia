using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;


namespace Reia
{
    class Program
    {
        private static DiscordClient _discord;
        private static CommandsNextModule _commands;
        private static Entities.Config _config;
        public static volatile HttpClient Client = new HttpClient();

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static void WriteCenter(string value, int skipline = 0)
        {
            for (int i = 0; i < skipline; i++)
            {
                Console.WriteLine();
            }
            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
        }


        static void InitModules(Entities.Config config)
        {
            Commands.ModuleApiYoutube.Init(config.ApiYoutubeKey);
        }

        static async Task MainAsync(string[] args)
        {
            // Need to load config
            if (!File.Exists("config.json"))
            {
                new Entities.ConfigReader().SaveToFile("config.json");
                #region !! Report to user that config has not been set yet!! (aesthetics)
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                WriteCenter("▒▒▒▒▒▒▒▒▒▄▄▄▄▒▒▒▒▒▒▒", 2);
                WriteCenter("▒▒▒▒▒▒▄▀▀▓▓▓▀█▒▒▒▒▒▒");
                WriteCenter("▒▒▒▒▄▀▓▓▄██████▄▒▒▒▒");
                WriteCenter("▒▒▒▄█▄█▀░░▄░▄░█▀▒▒▒▒");
                WriteCenter("▒▒▄▀░██▄░░▀░▀░▀▄▒▒▒▒");
                WriteCenter("▒▒▀▄░░▀░▄█▄▄░░▄█▄▒▒▒");
                WriteCenter("▒▒▒▒▀█▄▄░░▀▀▀█▀▒▒▒▒▒");
                WriteCenter("▒▒▒▄▀▓▓▓▀██▀▀█▄▀▀▄▒▒");
                WriteCenter("▒▒█▓▓▄▀▀▀▄█▄▓▓▀█░█▒▒");
                WriteCenter("▒▒▀▄█░░░░░█▀▀▄▄▀█▒▒▒");
                WriteCenter("▒▒▒▄▀▀▄▄▄██▄▄█▀▓▓█▒▒");
                WriteCenter("▒▒█▀▓█████████▓▓▓█▒▒");
                WriteCenter("▒▒█▓▓██▀▀▀▒▒▒▀▄▄█▀▒▒");
                WriteCenter("▒▒▒▀▀▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒");
                Console.BackgroundColor = ConsoleColor.Yellow;
                WriteCenter("WARNING", 3);
                Console.ResetColor();
                WriteCenter("Thank you Mario!", 1);
                WriteCenter("But our config.json is in another castle!");
                WriteCenter("(Please fill in the config.json that was generated.)", 2);
                WriteCenter("Press any key to exit..", 1);
                Console.SetCursorPosition(0, 0);
                Console.ReadKey();
                #endregion
                Environment.Exit(0);

            }

            _config = Entities.ConfigReader.LoadFromFile("config.json");

            // Create the instance bot with the bot token
            _discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = _config.BotToken,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            InitModules(_config);

            // Create and Initialise the command instance to accept commands with prefix string
            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = _config.BotPrefix
            });
            // Initialise it
            _commands.RegisterCommands<CustomCommands>();
            // Connect the bot to the server, asynchronously
            await _discord.ConnectAsync();
            // Infinite wait time to keep the bot running
            await Task.Delay(-1);
        }
    }
}
