using Domain.Contracts;
using Domain.Data;
using Microsoft.Extensions.DependencyInjection;
using MockRealiztions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botId = Environment.GetEnvironmentVariable("BotToken");

var serviceProvider = ConfigureServices();

var botClient = new TelegramBotClient(botId);
using CancellationTokenSource cts = new();

var chats = new Dictionary<long, ChatData>();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};


botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);
Console.ReadLine();


static IServiceProvider ConfigureServices()
{
    var services = new ServiceCollection();
    services.AddSingleton<IConversationAgent, MockConversationalAgent>();
    return services.BuildServiceProvider();
}
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var msg = update.Message;
    if (msg == null)
        return;

    var chatId = update.Message.Chat.Id;

    if (!chats.ContainsKey(chatId))
    {
        chats[chatId] = new ChatData() { ChatId = chatId, Stage = ProcessStage.Start };
    }

    var chatData = chats[chatId];

    await ProcessCurrentStep(chatData, msg, botClient, cts.Token);


    await botClient.SendTextMessageAsync(chatId, "echo");
    return;
}

async Task ProcessCurrentStep(ChatData chatData, Message msg, ITelegramBotClient botClient, CancellationToken cancellationToken)
{
    var conversationalAgent = serviceProvider.GetRequiredService<IConversationAgent>();
    switch (chatData.Stage)
    {
        case ProcessStage.Start:
            await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.Greet());
            await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForPassport());
            chatData.Stage = ProcessStage.WaitingForPassportPhoto;
            break;
        case ProcessStage.WaitingForPassportPhoto:
            if (msg.Photo == null)
            {
                await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.ErrorMessage());
                await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForPassport());
                return;
            } // TODO : Change errors to more conditional depends on stage.

            var photoId = msg.Photo.Last().FileId;

            var photoFileInfo = await botClient.GetFileAsync(photoId);
            var path = photoFileInfo.FilePath;

            const string destinationFilePath = "../downloaded.file";

            Stream fileStream = new MemoryStream();
            await botClient.DownloadFileAsync(
                filePath: path,
                destination: fileStream,
                cancellationToken: cancellationToken);



            break;
        case ProcessStage.WaitingForPassportDataApprove:
            break;
        case ProcessStage.WaitingForVenichleIdPhoto:
            break;
        case ProcessStage.WaitingForVenichleIdDataApprove:
            break;
        case ProcessStage.WaitingForPriceConfirmation:
            break;
    }

}
Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ErrorMessage);
    Console.ForegroundColor = ConsoleColor.White;

    return Task.CompletedTask;
}

public enum ProcessStage
{
    Start,
    WaitingForPassportPhoto,
    WaitingForPassportDataApprove,
    WaitingForVenichleIdPhoto,
    WaitingForVenichleIdDataApprove,
    WaitingForPriceConfirmation,
}

public class ChatData
{
    public long ChatId { get; set; }
    public ProcessStage Stage { get; set; }

    public PassportData? PassportData { get; set; }
    public VenicleIdData? VenicleIdData { get; set; }
}