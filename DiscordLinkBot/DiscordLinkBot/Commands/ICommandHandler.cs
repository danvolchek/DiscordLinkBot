using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    internal interface ICommandHandler
    {
        bool IsAdminOnlyCommand { get; }

        string Name { get; }

        string HelpText { get; }

        bool CanHandle(DiscordMessage message);

        string Handle(DiscordMessage message);
    }
}
