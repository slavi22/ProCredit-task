using ProCredit_task.Models;

namespace ProCredit_task.Contracts;

public interface IDatabase
{
    Task<MessageModel> GetLastAddedRow(long id);
    Task<MessageModel> UploadMessageToDatabase(Dictionary<string, string> message);
}