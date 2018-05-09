using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    /// Base command.
    /// </summary>
    internal abstract class BaseCommand : ICommand
    {
        public abstract bool IsAdminOnlyCommand { get; }
        public abstract string Name { get; }
        public abstract string HelpText { get; }

        public virtual bool CanHandle(DiscordMessage message)
        {
            string[] parts = message.Content.ToLowerInvariant().Split(' ');
            return parts.Length > 0 && parts[0] == $"{Program.CommandChar}{this.Name}";
        }

        public abstract string Handle(DiscordMessage message);
    }
}
