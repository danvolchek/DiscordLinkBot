using System.Collections.Generic;
using DiscordLinkBot.Commands;

namespace DiscordLinkBot
{
    /// <summary>
    ///     A manager of commands.
    /// </summary>
    internal interface ICommandManager
    {
        /// <summary>
        ///     Get the <see cref="ICommand.HelpText" /> for a given command name that this manager manages.
        /// </summary>
        string GetCommandHelpString(string name);

        /// <summary>
        ///     Get all command names that this manager manages.
        /// </summary>
        IEnumerable<string> GetCommandNames();
    }
}