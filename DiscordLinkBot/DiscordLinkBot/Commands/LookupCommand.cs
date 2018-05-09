using DSharpPlus.Entities;
using System.Data.SQLite;

namespace DiscordLinkBot.Commands
{
    internal class LookupCommand : DatabaseCommand
    {
        private string lastLookedUpMessage;

        public LookupCommand(SQLiteConnection connection) : base(connection)
        {
        }

        public override bool IsAdminOnlyCommand => false;

        public override string Name => "lookup";

        public override string HelpText => $"Replies with a premade message.\nUsage: `{Program.CommandChar}<name> [mentioned user]`.";

        public override bool CanHandle(DiscordMessage message)
        {
            string[] args = message.Content.Split(' ');

            switch (args.Length)
            {
                case 0:
                    return false;
                case 2:
                case 1:
                    if (!args[0].StartsWith(Program.CommandChar))
                        return false;

                    SQLiteCommand command = new SQLiteCommand("SELECT message FROM commands WHERE name=@name;", this.Connection);
                    command.Parameters.AddWithValue("@name", args[0].Substring(1).ToLowerInvariant());

                    return (this.lastLookedUpMessage = (string)command.ExecuteScalar()) != null;
                default:
                    return false;
            }

        }

        public override string Handle(DiscordMessage message)
        {
            string mention = message.MentionedUsers.Count > 0 ? $"{message.MentionedUsers[0].Mention}" : "";
            return $"{mention} {this.lastLookedUpMessage}";
        }
    }
}
