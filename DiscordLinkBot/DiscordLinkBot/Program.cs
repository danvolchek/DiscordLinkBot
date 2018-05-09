using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiscordLinkBot.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordLinkBot
{
    internal class Program : ICommandManager
    {
        internal const char CommandChar = '!';
        private IList<ICommandHandler> commandHandlers;
        private SQLiteConnection connection;
        private DiscordClient discord;

        public string GetCommandHelpString(string name)
        {
            name = name.ToLowerInvariant();
            return this.commandHandlers.FirstOrDefault(handler => handler.Name == name)?.HelpText;
        }

        public IEnumerable<string> GetCommandNames()
        {
            return this.commandHandlers.Select(handler => handler.Name);
        }

        private static void Main(string[] args)
        {
            new Program().MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task MainAsync(string[] args)
        {
            Console.Write("Opening connection... ");
            this.connection = new SQLiteConnection(@"DbLinqProvider=Sqlite;Data Source=database.db");
            this.connection.Open();
            Console.WriteLine("Okay!");

            Console.Write("Creating table... ");
            SQLiteCommand command =
                new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='commands';",
                    this.connection);
            if (command.ExecuteScalar() == null)
            {
                command.CommandText = "CREATE TABLE commands(name TEXT PRIMARY KEY, message TEXT);";
                command.ExecuteNonQuery();
                Console.WriteLine("Okay!");
            }
            else
            {
                Console.WriteLine("Already exists!");
            }

            Console.Write("Adding handlers... ");
            this.commandHandlers = new List<ICommandHandler>
            {
                new DefineCommand(this.connection),
                new RedefineCommand(this.connection),
                new DeleteCommand(this.connection),
                new ListAllCommand(this.connection),
                new HelpCommand(this.connection, this),
                new LookupCommand(this.connection)
            };
            Console.WriteLine("Okay!");

            Console.Write("Connecting to discord... ");
            this.discord = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllText("credentials.txt"),
                TokenType = TokenType.Bot
            });

            this.discord.MessageCreated += this.Discord_MessageCreated;

            await this.discord.ConnectAsync();
            Console.WriteLine("Okay!");

            Console.WriteLine("Listening... ");
            await Task.Delay(-1);

            Console.Write("Closing DB connection... ");
            this.connection.Close();
            Console.WriteLine("Okay!");
        }

        private async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Message.Author.IsBot)
                return;

            bool isAdmin = e.Guild.Members.FirstOrDefault(member => member.Id == e.Author.Id)?.Roles
                               .Any(role => role.Name.ToLowerInvariant().StartsWith("admin")) ?? false;

            ICommandHandler commandHandler = this.commandHandlers.FirstOrDefault(handler =>
                (!handler.IsAdminOnlyCommand || isAdmin) && handler.CanHandle(e.Message));

            string result = commandHandler?.Handle(e.Message);
            if (result != null)
                await e.Message.RespondAsync(result);
        }
    }
}