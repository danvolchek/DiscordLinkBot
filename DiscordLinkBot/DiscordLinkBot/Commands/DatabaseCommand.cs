using System.Data.SQLite;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    ///     Command which requires a database connection.
    /// </summary>
    internal abstract class DatabaseCommand : BaseCommand
    {
        protected readonly SQLiteConnection Connection;

        protected DatabaseCommand(SQLiteConnection connection)
        {
            this.Connection = connection;
        }
    }
}