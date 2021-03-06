﻿using System.Data.SQLite;
using System.Linq;
using DSharpPlus.Entities;

namespace DiscordLinkBot.Commands
{
    /// <summary>
    ///     Command to delete existing commands.
    /// </summary>
    internal class DeleteCommand : DatabaseCommand
    {
        public DeleteCommand(SQLiteConnection connection) : base(connection)
        {
        }

        public override bool IsAdminOnlyCommand => true;

        public override string Name => "delete";

        public override string HelpText =>
            $"Deletes an existing command.\nUsage: `{Program.CommandChar}{this.Name} <name>`.";

        public override string Handle(DiscordMessage message)
        {
            string[] args = message.Content.Split(' ').Skip(1).ToArray();

            switch (args.Length)
            {
                case 0:
                    return "You need to provide a name for this command!";
                case 1:
                    SQLiteCommand command =
                        new SQLiteCommand("SELECT name FROM commands WHERE name=@name;", this.Connection);
                    command.Parameters.AddWithValue("@name", args[0].ToLowerInvariant());
                    if (command.ExecuteScalar() == null) return "That command doesn't exist!";

                    command.CommandText = "DELETE FROM commands WHERE name=@name;";

                    return command.ExecuteNonQuery() == 0 ? "Failure deleting command from db :(" : "Command deleted!";
                default:
                    return $"Invalid args. See `{Program.CommandChar}help {this.Name} for usage`.";
            }
        }
    }
}