using DSharpPlus.Entities;
using System.Data.SQLite;
using System.Linq;

namespace DiscordLinkBot.Commands
{
    internal class DefineCommand : DatabaseCommand
    {
        public DefineCommand(SQLiteConnection connection) : base(connection)
        {
        }

        public override bool IsAdminOnlyCommand => true;

        public override string Name => "define";

        public override string HelpText => $"Defines a new command.\nUsage: `{Program.CommandChar}{this.Name} <name> <message>`.";

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
                    SQLiteCommand command = new SQLiteCommand("SELECT name FROM commands WHERE name=@name;", this.Connection);
                    command.Parameters.AddWithValue("@name", args[0]);
                    if (command.ExecuteScalar() != null)
                    {
                        return "That command already exists!";
                    }

                    command.CommandText = "INSERT INTO commands VALUES (@name, @message);";
                    command.Parameters.AddWithValue("@message", string.Join(" ", args.Skip(1)));

                    return command.ExecuteNonQuery() == 0 ? "DB Failure adding command :(" : $"Command added! You can use it like `{Program.CommandChar}{args[0]}` or `{Program.CommandChar}{args[0]} user#0000`!";
            }
        }
    }
}
