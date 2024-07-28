using System.Data;
using Microsoft.Data.Sqlite;
using ProCredit_task.Contracts;

namespace ProCredit_task.Services;

public class Database : IDatabase
{
    private readonly string _connectionString;
    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<string> GetAllMessages()
    {
        await using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            await using (var cmd = new SqliteCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM messages";

                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Console.WriteLine($"id-{reader["id"]}; message-{reader["message"]}");
                }
            }
        }

        return "";
    }
}