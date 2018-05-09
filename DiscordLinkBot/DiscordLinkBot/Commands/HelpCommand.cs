using DSharpPlus.Entities;
using System.Data.SQLite;
using System.Linq;

namespace DiscordLinkBot.Commands
{
    internal class HelpCommand : DatabaseCommand
    {
        private readonly ICommandManager manager;

        public HelpCommand(SQLiteConnection connection, ICommandManager manager) : base(connection)
        {
            this.manager = manager;
        }

        public override bool IsAdminOnlyCommand => false;
        public override string Name => "help";

        public override string HelpText =>
            $"Gives information about existing commands.\nUsage `{Program.CommandChar}{this.Name} [command name]`.";

        public override string Handle(DiscordMessage message)
        {
            string[] args = message.Content.Split(' ').Skip(1).ToArray();

            switch (args.Length)
            {
                case 0:
                    return $"Available commands:\n{string.Join(", ", this.manager.GetCommandNames())}\nUse `{Program.CommandChar}{this.Name} <name>` to see more info!";
                case 1:
                    return this.manager.GetCommandHelpString(args[0]) ?? "That command name doesn't exist!";
                default:
                    return $"Invalid args. See `{Program.CommandChar}help {this.Name} for usage`.";
            }
        }
    }
}
