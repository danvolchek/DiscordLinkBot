using System.Data.SQLite;
using System.Linq;
using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    /// Command to redefine existing commands.
    /// </summary>
    internal class RedefineCommand : DatabaseCommand
    {
        public RedefineCommand(SQLiteConnection connection) : base(connection)
        {
        }

        public override bool IsAdminOnlyCommand => true;

        public override string Name => "redefine";

        public override string HelpText =>
            $"Redefines an existing command.\nUsage: `{Program.CommandChar}{this.Name} <name> <message>`.";

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
                    SQLiteCommand command =
                        new SQLiteCommand("SELECT name FROM commands WHERE name=@name;", this.Connection);
                    command.Parameters.AddWithValue("@name", args[0].ToLowerInvariant());
                    if (command.ExecuteScalar() == null)
                        return $"That command doesn't exist! Use `{Program.CommandChar}define` to define it.";

                    command.CommandText = "UPDATE commands SET message=@message WHERE name=@name;";
                    command.Parameters.AddWithValue("@message", string.Join(" ", args.Skip(1)));

                    return command.ExecuteNonQuery() == 0 ? "DB failure redefining command :(" : "Command redefined!";
            }
        }
    }
}