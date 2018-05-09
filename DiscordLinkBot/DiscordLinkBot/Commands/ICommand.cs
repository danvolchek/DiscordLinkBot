using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    ///     A chat command.
    /// </summary>
    internal interface ICommand
    {
        /// <summary>
        ///     Whether the command requires an admin role to use.
        /// </summary>
        bool IsAdminOnlyCommand { get; }

        /// <summary>
        ///     The name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The help text of the command.
        /// </summary>
        string HelpText { get; }

        /// <summary>
        ///     Whether the command can handle the given message.
        /// </summary>
        bool CanHandle(DiscordMessage message);

        /// <summary>
        ///     Handle the given message and return a reply string.
        /// </summary>
        string Handle(DiscordMessage message);
    }
}