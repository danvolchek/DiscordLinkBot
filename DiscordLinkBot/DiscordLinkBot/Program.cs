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
        private IList<ICommand> commands;
        private SQLiteConnection connection;
        private DiscordClient discord;

        /// <summary>
        /// Get the <see cref="ICommand.HelpText"/> for a given command name that this manager manages.
        /// </summary>
        public string GetCommandHelpString(string name)
        {
            name = name.ToLowerInvariant();
            return this.commands.FirstOrDefault(handler => handler.Name == name)?.HelpText;
        }

        /// <summary>
        /// Get all command names that this manager manages.
        /// </summary>
        public IEnumerable<string> GetCommandNames()
        {
            return this.commands.Select(handler => handler.Name);
        }

        /// <summary>
        /// Program entry point. Starts the <see cref="MainAsync"/> task.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            new Program().MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Main task. Sets up the DB, commands, and then connects to discord.
        /// </summary>
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
            this.commands = new List<ICommand>
            {
                new DefineCommand(this.connection, this),
                new RedefineCommand(this.connection),
                new DeleteCommand(this.connection),
                new ListCommand(this.connection),
                new HelpCommand(this),
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

        /// <summary>
        /// Event handler for new messages. Handles the message using a <see cref="ICommand"/>.
        /// </summary>
        private async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Message.Author.IsBot)
                return;

            bool isAdmin = e.Guild.Members.FirstOrDefault(member => member.Id == e.Author.Id)?.Roles
                               .Any(role => role.Name.ToLowerInvariant().StartsWith("admin")) ?? false;

            ICommand command = this.commands.FirstOrDefault(handler =>
                (!handler.IsAdminOnlyCommand || isAdmin) && handler.CanHandle(e.Message));

            string result = command?.Handle(e.Message);
            if (result != null)
                await e.Message.RespondAsync(result);
        }
    }
}