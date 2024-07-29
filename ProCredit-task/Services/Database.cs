using System.Data;
using Microsoft.Data.Sqlite;
using ProCredit_task.Contracts;
using ProCredit_task.Models;

namespace ProCredit_task.Services;

public class Database : IDatabase
{
    private readonly ILogger<Database> _logger;
    private readonly string _connectionString;

    public Database(ILogger<Database> logger, string connectionString)
    {
        _logger = logger;
        _logger.LogDebug("NLog injected into Database");
        _connectionString = connectionString;
    }

    public async Task<MessageModel> GetLastAddedRow(long id)
    {
        var messageModel = new MessageModel();
        await using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            await using (var cmd = new SqliteCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM swift_messages WHERE id = @id";
                cmd.Parameters.AddWithValue("id", id);
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    messageModel.Id = (long)reader["id"];
                    messageModel.BasicHeaderInfo = (string)reader["basic_header_info"];
                    messageModel.ApplicationHeaderInfo = (string)reader["application_header_info"];
                    messageModel.RelatedRef = (string)reader["related_ref"];
                    messageModel.Narrative = (string)reader["narrative"];
                    messageModel.MAC = (string)reader["mac"];
                    messageModel.CHK = (string)reader["chk"];
                }
            }
        }

        return messageModel;
    }

    public async Task<MessageModel> UploadMessageToDatabase(Dictionary<string, string> message)
    {
        _logger.LogDebug("Uploading the message fields to the database.");
        var messageModel = new MessageModel();
        await using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            await using (var cmd = new SqliteCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO " +
                                  "swift_messages(basic_header_info, application_header_info, related_ref, narrative, mac, chk) " +
                                  "VALUES(@BasicHeaderInfo, @ApplicationHeaderInfo, @RelatedRef, @Narrative, @MAC, @CHK); " +
                                  "SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("BasicHeaderInfo", message["BasicHeaderInfo"]);
                cmd.Parameters.AddWithValue("ApplicationHeaderInfo", message["ApplicationHeaderInfo"]);
                cmd.Parameters.AddWithValue("RelatedRef", message["RelatedRef"]);
                cmd.Parameters.AddWithValue("Narrative", message["Narrative"]);
                cmd.Parameters.AddWithValue("MAC", message["MAC"]);
                cmd.Parameters.AddWithValue("CHK", message["CHK"]);
                var addedRowId = Convert.ToInt64(cmd.ExecuteScalar());
                messageModel = await GetLastAddedRow(addedRowId);
            }
        }
        _logger.LogDebug("Message successfully added to the database");
        return messageModel;
    }
}