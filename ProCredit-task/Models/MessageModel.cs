namespace ProCredit_task.Models;

public class MessageModel
{
    public long Id { get; set; }
    public string BasicHeaderInfo { get; set; }
    public string ApplicationHeaderInfo { get; set; }
    public string RelatedRef { get; set; }
    public string Narrative { get; set; }
    public string MAC { get; set; }
    public string CHK { get; set; }
}