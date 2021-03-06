﻿using System.Data.SQLite;
using System.Linq;
using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    ///     Command to list all commands.
    /// </summary>
    internal class ListCommand : DatabaseCommand
    {
        public ListCommand(SQLiteConnection connection) : base(connection)
        {
        }

        public override bool IsAdminOnlyCommand => false;

        public override string Name => "list";

        public override string HelpText =>
            $"Lists all command names.\nUsage: `{Program.CommandChar}list [page number]`.";

        public override string Handle(DiscordMessage message)
        {
            string[] args = message.Content.Split(' ').Skip(1).ToArray();

            int offset = 0;

            switch (args.Length)
            {
                case 0:
                    break;
                case 1 when int.TryParse(args[0], out int pageNumber):
                    offset = 10 * pageNumber;
                    break;
                default:
                    return null;
            }

            SQLiteCommand command =
                new SQLiteCommand("SELECT name FROM commands LIMIT 10 OFFSET @offset;", this.Connection);
            command.Parameters.AddWithValue("@offset", offset);

            string result = "Commands: ";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read()) result += $"{reader.GetString(0)}, ";
            }

            return result;
        }
    }
}