using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    /// A chat command.
    /// </summary>
    internal interface ICommand
    {
        /// <summary>
        /// Whether the command requires an admin role to use.
        /// </summary>
        bool IsAdminOnlyCommand { get; }

        /// <summary>
        /// Command name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Command help text.
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// Whether this command can handle the given message.
        /// </summary>
        bool CanHandle(DiscordMessage message);

        /// <summary>
        /// Handle the given message and return a reply string.
        /// </summary>
        string Handle(DiscordMessage message);
    }
}