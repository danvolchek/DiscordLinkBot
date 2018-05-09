using System.Data.SQLite;
using System.Linq;
using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    /// Command to define new commands.
    /// </summary>
    internal class DefineCommand : DatabaseCommand
    {
        private readonly ICommandManager manager;

        public DefineCommand(SQLiteConnection connection, ICommandManager manager) : base(connection)
        {
            this.manager = manager;
        }

        public override bool IsAdminOnlyCommand => true;

        public override string Name => "define";

        public override string HelpText =>
            $"Defines a new command.\nUsage: `{Program.CommandChar}{this.Name} <name> <message>`.";

        public override string Handle(DiscordMessage message)
        {
            string[] args = message.Content.Split(' ').Skip(1).ToArray();

            switch (args.Length)
            {
                case 0:
                    return "You need to provide a name for this command!";
                case 1:
                    return "You need to provide a message for this command!";
                default:
                    string commandName = args[0].ToLowerInvariant();

                    SQLiteCommand command =
                        new SQLiteCommand("SELECT name FROM commands WHERE name=@name;", this.Connection);
                    command.Parameters.AddWithValue("@name", commandName);

                    if (command.ExecuteScalar() != null) return "That command already exists!";

                    if (this.manager.GetCommandNames().Any(name => name == commandName))
                        return $"{args[0]} is a reserved name, please use a different name!";

                    command.CommandText = "INSERT INTO commands VALUES (@name, @message);";
                    command.Parameters.AddWithValue("@message", string.Join(" ", args.Skip(1)));

                    return command.ExecuteNonQuery() == 0
                        ? "DB Failure adding command :("
                        : $"Command added! You can use it like `{Program.CommandChar}{args[0]}` or `{Program.CommandChar}{args[0]} user#0000`!";
            }
        }
    }
}