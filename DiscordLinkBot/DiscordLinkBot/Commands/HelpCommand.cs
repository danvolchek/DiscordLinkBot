using System.Linq;
using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    ///     Command to get help about other commands.
    /// </summary>
    internal class HelpCommand : BaseCommand
    {
        private readonly ICommandManager manager;

        public HelpCommand(ICommandManager manager)
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
                    return
                        $"Available commands:\n{string.Join(", ", this.manager.GetCommandNames())}\nUse `{Program.CommandChar}{this.Name} <name>` to see more info!";
                case 1:
                    return this.manager.GetCommandHelpString(args[0].ToLowerInvariant()) ??
                           "That command name doesn't exist!";
                default:
                    return $"Invalid args. See `{Program.CommandChar}help {this.Name} for usage`.";
            }
        }
    }
}