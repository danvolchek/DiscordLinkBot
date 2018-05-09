using System.Data.SQLite;
using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    internal abstract class DatabaseCommand : ICommandHandler
    {
        protected readonly SQLiteConnection Connection;

        protected DatabaseCommand(SQLiteConnection connection)
        {
            this.Connection = connection;
        }

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