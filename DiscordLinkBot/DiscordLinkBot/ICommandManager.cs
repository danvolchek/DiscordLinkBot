using System.Collections.Generic;

namespace DiscordLinkBot
{
    internal interface ICommandManager
    {
        string GetCommandHelpString(string name);

        IEnumerable<string> GetCommandNames();
    }
}