using BotApplication;
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
using Telegram.Bot.Types.ReplyMarkups;

var botId = Environment.GetEnvironmentVariable("BotToken");

var serviceProvider = ConfigureServices();

var botClient = new TelegramBotClient(botId);
using CancellationTokenSource cts = new();

var chats = new Dictionary<long, ChatData>();
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

async Task ProcessInlineKeyboardAnswer(string arg1, string? arg2, bool? nullable1, string? arg4, int? nullable2, CancellationToken token)
{
    throw new NotImplementedException();
}

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
    services.AddSingleton<IPictureProcessor, MockPictureProcessor>();
    services.AddSingleton<ILogger, ConsoleLogger>();
    return services.BuildServiceProvider();
}
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    long chatId;
    ChatData chatData;
    var msg = update.Message;
    var callBack = update.CallbackQuery;

    if (callBack != null)
    {

        chatId = callBack.Message.Chat.Id;
        if (!chats.ContainsKey(chatId))
            throw new Exception("Got callback data from inline answer from unregistered chat");

        chatData = chats[chatId];
        await ProcessReply(chatData, callBack, botClient, cts.Token);
    }


    chatId = update.Message.Chat.Id;
    if (msg == null)
        return;


    if (!chats.ContainsKey(chatId))
    {
        chats[chatId] = new ChatData() { ChatId = chatId, Stage = ProcessStage.Start };
    }

    chatData = chats[chatId];

    await ProcessCurrentStep(chatData, msg, botClient, cts.Token);
    return;
}

async Task ProcessReply(ChatData chatData, CallbackQuery callBack, ITelegramBotClient botClient, CancellationToken token)
{

    bool parsingSuccess = Enum.TryParse(typeof(InlineReplies), callBack.Data, out var commandId);
    if (!parsingSuccess)
        throw new Exception($"Could not parse callback data to inline replies. Callback data : {callBack.Data}");

    switch ((InlineReplies)commandId)
    {
        case InlineReplies.PassportConfirm:
            break;
        case InlineReplies.PassportUnconfirm:
            break;
        case InlineReplies.VenichleIdConfirm:
            break;
        case InlineReplies.VenichleIdUnconfirm:
            break;
        case InlineReplies.PriceConfirm:
            break;
        case InlineReplies.PriceUnconfirm:
            break;
    }

}

async Task ProcessCurrentStep(ChatData chatData, Message msg, ITelegramBotClient botClient, CancellationToken cancellationToken)
{
    var conversationalAgent = serviceProvider.GetRequiredService<IConversationAgent>();
    var picProcessAgent = serviceProvider.GetRequiredService<IPictureProcessor>();

    string photoId, filePath;
    Telegram.Bot.Types.File fileInfo;
    MemoryStream fileStream;

    switch (chatData.Stage)
    {
        case ProcessStage.Start:
            await BotStartConversation(chatData, botClient, conversationalAgent);
            break;

        case ProcessStage.WaitingForPassportPhoto:
            await ProcessPassportPhoto(chatData, msg, botClient, conversationalAgent, picProcessAgent, cancellationToken);
            break;

        case ProcessStage.WaitingForPassportDataApprove:
            await ReAskForPassportDataApprove(chatData, botClient, conversationalAgent);
            // Asking for approve in case user send anything but not using buttons "Yes/No". Resend the question and buttons
            break;

        case ProcessStage.WaitingForVenichleIdPhoto:
            await ProcessVenichleIdPhoto(chatData, msg, botClient, conversationalAgent, picProcessAgent, cancellationToken);
            break;

        case ProcessStage.WaitingForVenichleIdDataApprove:
            await ReAskForVenichleIdDataApprove(chatData, botClient, conversationalAgent);
            // Asking for approve in case user send anything but not using buttons "Yes/No". Resend the question and buttons
            break;

        case ProcessStage.WaitingForPriceConfirmation:
            await ReAskForPriceApprove(chatData, botClient, conversationalAgent);

            break;
    }
}
Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var logger = serviceProvider.GetRequiredService<ILogger>();
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    logger.LogError(ErrorMessage);

    return Task.CompletedTask;
}

static async Task BotStartConversation(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.Greet());
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForPassport());
    chatData.Stage = ProcessStage.WaitingForPassportPhoto;
}
static async Task ProcessPassportPhoto(ChatData chatData, Message msg, ITelegramBotClient botClient, IConversationAgent conversationalAgent, IPictureProcessor picProcessAgent, CancellationToken cancellationToken)
{
    if (msg.Photo == null)
    {
        await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.ErrorMessage());
        await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForPassport());
        return;
    } // TODO : Change errors to more conditional depends on stage.

    var photoId = msg.Photo.Last().FileId;

    var fileInfo = await botClient.GetFileAsync(photoId);
    var path = fileInfo.FilePath;

    PassportData passportData;
    using (var fileStream = new MemoryStream())
    {
        await botClient.DownloadFileAsync(
            filePath: path,
            destination: fileStream,
            cancellationToken: cancellationToken);

        passportData = picProcessAgent.ProcessPassportPicture(fileStream);
    }
    chatData.PassportData = passportData;

    var acceptButton = InlineKeyboardButton.WithCallbackData("Yes", InlineReplies.PassportConfirm.ToString());
    var rejectButton = InlineKeyboardButton.WithCallbackData("No", InlineReplies.PassportConfirm.ToString());

    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForPassportApprove(passportData), replyMarkup: new InlineKeyboardMarkup([acceptButton, rejectButton]));

    chatData.Stage = ProcessStage.WaitingForPassportDataApprove;
}
static async Task ProcessVenichleIdPhoto(ChatData chatData, Message msg, ITelegramBotClient botClient, IConversationAgent conversationalAgent, IPictureProcessor picProcessAgent, CancellationToken cancellationToken)
{
    if (msg.Photo == null)
    {
        await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.ErrorMessage());
        await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForVenichleId());
        return;
    } // TODO : Change errors to more conditional depends on stage.

    var photoId = msg.Photo.Last().FileId;

    var fileInfo = await botClient.GetFileAsync(photoId);
    var path = fileInfo.FilePath;

    VenicleIdData venichleIdData;
    using (var fileStream = new MemoryStream())
    {
        await botClient.DownloadFileAsync(
            filePath: path,
            destination: fileStream,
            cancellationToken: cancellationToken);

        venichleIdData = picProcessAgent.ProcessVenichleIdPicture(fileStream);
    }
    chatData.VenicleIdData = venichleIdData;

    var acceptButton = InlineKeyboardButton.WithCallbackData("Yes", InlineReplies.PassportConfirm.ToString());
    var rejectButton = InlineKeyboardButton.WithCallbackData("No", InlineReplies.PassportConfirm.ToString());

    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForVenichleIdApprove(venichleIdData), replyMarkup: new InlineKeyboardMarkup([acceptButton, rejectButton]));

    chatData.Stage = ProcessStage.WaitingForPassportDataApprove;
}

static async Task ReAskForPassportDataApprove(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.ErrorMessage());
    var acceptButton = InlineKeyboardButton.WithCallbackData("Yes", InlineReplies.PassportConfirm.ToString());
    var rejectButton = InlineKeyboardButton.WithCallbackData("No", InlineReplies.PassportUnconfirm.ToString());
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForPassportApprove(chatData.PassportData), replyMarkup: new InlineKeyboardMarkup([acceptButton, rejectButton]));
}
static async Task ReAskForVenichleIdDataApprove(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.ErrorMessage());
    var acceptButton = InlineKeyboardButton.WithCallbackData("Yes", InlineReplies.VenichleIdConfirm.ToString());
    var rejectButton = InlineKeyboardButton.WithCallbackData("No", InlineReplies.VenichleIdUnconfirm.ToString());
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.AskForVenichleIdApprove(chatData.VenicleIdData), replyMarkup: new InlineKeyboardMarkup([acceptButton, rejectButton]));
}
static async Task ReAskForPriceApprove(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.ErrorMessage());
    var acceptButton = InlineKeyboardButton.WithCallbackData("Yes", InlineReplies.PriceConfirm.ToString());
    var rejectButton = InlineKeyboardButton.WithCallbackData("No", InlineReplies.PriceUnconfirm.ToString());
    await botClient.SendTextMessageAsync(chatData.ChatId, conversationalAgent.PriceAnnouncement(), replyMarkup: new InlineKeyboardMarkup([acceptButton, rejectButton]));
}
