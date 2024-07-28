namespace ProCredit_task.Contracts;

public interface IDatabase
{
    Task<string> GetAllMessages();
}