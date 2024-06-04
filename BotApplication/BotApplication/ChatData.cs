﻿using Domain.Data;

public class ChatData
{
    public long ChatId { get; set; }
    public ProcessStage Stage { get; set; }

    public PassportData? PassportData { get; set; }
    public VenicleIdData? VenicleIdData { get; set; }
}