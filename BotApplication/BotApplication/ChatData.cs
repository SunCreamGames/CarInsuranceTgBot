using BotApplication;
using Domain.Data;

public class ChatData
{
    public long ChatId { get; set; }
    public ProcessStage Stage { get; set; }

    public PassportData? PassportData { get; set; }
    public VeniclePlateData? VenicleIdData { get; set; }
}