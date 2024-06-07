using BotApplication;
using Domain.Contracts;
using Domain.Data;
using Microsoft.Extensions.DependencyInjection;
using MindeePictureProcessing;
using MockRealiztions;
using OpenAiClient;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


var serviceProvider = ConfigureServices();

var logger = serviceProvider.GetRequiredService<ILogger>();
logger.LogMessage("Bot starting");


var botId = Environment.GetEnvironmentVariable("BotToken");
var botClient = new TelegramBotClient(botId);

using CancellationTokenSource cts = new();

var chats = new Dictionary<long, ChatData>();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

var conversationalAgent = serviceProvider.GetRequiredService<IConversationAgent>();
var policyGenerator = serviceProvider.GetRequiredService<IPolicyGenerator>();


await conversationalAgent.Init();
await policyGenerator.InitTemplate();

logger.LogMessage("Bot started listening");
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
    //services.AddSingleton<IConversationAgent, MockConversationalAgent>();
    //services.AddSingleton<IPictureProcessor, MockPictureProcessor>();
    //services.AddSingleton<IPolicyGenerator, DummyPolicyGenerator>();

    services.AddSingleton<IConversationAgent, OpenAiConversationalAgent>();
    services.AddSingleton<IPictureProcessor, MindeePictureProcessor>();
    services.AddSingleton<IPolicyGenerator, OpenAiPdfFileGenerator>();

    services.AddSingleton<ILogger, ConsoleLogger>();
    return services.BuildServiceProvider();
}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    long chatId;
    ChatData chatData;
    var msg = update.Message;
    var callBack = update.CallbackQuery;

    // Process button answer 'yes' or 'no' in confirmation steps
    if (callBack != null)
    {
        chatId = callBack.Message.Chat.Id;
        if (!chats.ContainsKey(chatId))
            return;
        //throw new Exception("Got callback data from inline answer from unregistered chat");

        chatData = chats[chatId];
        await ProcessButtonReply(chatData, callBack, botClient, cts.Token);
        return;
    }

    if (msg == null)
        return;

    chatId = msg.Chat.Id;


    if (!chats.ContainsKey(chatId))
    {
        chats[chatId] = new ChatData() { ChatId = chatId, Stage = ProcessStage.Start };
    }

    chatData = chats[chatId];

    await ProcessCurrentStep(chatData, msg, botClient, cts.Token);
    return;
}

async Task ProcessButtonReply(ChatData chatData, CallbackQuery callBack, ITelegramBotClient botClient, CancellationToken token)
{
    bool parsingSuccess = Enum.TryParse(typeof(InlineReplies), callBack.Data, out var commandId);
    if (!parsingSuccess)
    {
        logger.LogError($"Could not parse callback data to inline replies. Callback data : {callBack.Data}");
        throw new Exception($"Could not parse callback data to inline replies. Callback data : {callBack.Data}");
    }
    var replyCommand = (InlineReplies)commandId;

    var policyGenerator = serviceProvider.GetRequiredService<IPolicyGenerator>();

    var replyMarkUpWitoutButtons = new InlineKeyboardMarkup(new List<InlineKeyboardButton>());
    await botClient.EditMessageTextAsync(chatData.ChatId, callBack.Message.MessageId, callBack.Message.Text, replyMarkup: replyMarkUpWitoutButtons);

    switch (chatData.Stage)
    {
        case ProcessStage.WaitingForPassportDataApprove:
            if (replyCommand == InlineReplies.Confirm)
            {
                chatData.Stage = ProcessStage.WaitingForLicensePlatePhoto;
                await botClient.SendTextMessageAsync(
                    chatData.ChatId,
                   await conversationalAgent.AskForLicensePlateId());
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatData.ChatId,
                  await conversationalAgent.AskForPassportAgain());
                chatData.Stage = ProcessStage.WaitingForPassportPhoto;
            }
            break;
        case ProcessStage.WaitingForLicensePlateDataApprove:
            if (replyCommand == InlineReplies.Confirm)
            {
                chatData.Stage = ProcessStage.WaitingForPriceApprove;
                await AskGeneric(chatData.ChatId, botClient, await conversationalAgent.PriceAnnouncement(100));
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatData.ChatId,
                   await conversationalAgent.AskForLicensePlateIdAgain());
                chatData.Stage = ProcessStage.WaitingForLicensePlatePhoto;

            }
            break;
        case ProcessStage.WaitingForPriceApprove:
            if (replyCommand == InlineReplies.Confirm)
            {
                try
                {
                    var policy = await policyGenerator.CreateNewPolicy(chatData.PassportData, chatData.VenicleIdData, 100);

                    var stream = new MemoryStream();
                    stream.Write(policy);
                    stream.Position = 0;
                    InputFileStream fs = new InputFileStream(stream, $"policy for {chatData.PassportData.Name}.pdf");
                    await botClient.SendDocumentAsync(chatData.ChatId, fs);
                    chatData.Stage = ProcessStage.Start;
                }
                catch (Exception e)
                {
                    await botClient.SendTextMessageAsync(chatData.ChatId, e.Message);

                }
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatData.ChatId,
await conversationalAgent.RejectingPriceReaction(100));
                chatData.Stage = ProcessStage.Start;

            }
            break;
        default: return;
    }

}

async Task ProcessCurrentStep(ChatData chatData, Message msg, ITelegramBotClient botClient, CancellationToken cancellationToken)
{
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
            await ProcessPassportPhoto(chatData, msg, botClient, conversationalAgent, picProcessAgent, logger, cancellationToken);
            break;

        case ProcessStage.WaitingForPassportDataApprove:

            await ReAskForPassportDataApprove(chatData, botClient, conversationalAgent);
            // Asking for approve in case user send anything but not using buttons "Yes/No". Resend the question and buttons
            break;

        case ProcessStage.WaitingForLicensePlatePhoto:
            await ProcessLicensePlatePhoto(chatData, msg, botClient, conversationalAgent, picProcessAgent, logger, cancellationToken);
            break;

        case ProcessStage.WaitingForLicensePlateDataApprove:
            await ReAskForLicensePlateDataApprove(chatData, botClient, conversationalAgent);
            // Asking for approve in case user send anything but not using buttons "Yes/No". Resend the question and buttons
            break;

        case ProcessStage.WaitingForPriceApprove:
            await ReAskForPriceApprove(chatData, botClient, conversationalAgent);

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

    logger.LogError(ErrorMessage);

    return Task.CompletedTask;
}

static async Task BotStartConversation(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.Greet());
    await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.AskForPassport());
    chatData.Stage = ProcessStage.WaitingForPassportPhoto;
}
static async Task ProcessPassportPhoto(ChatData chatData, Message msg, ITelegramBotClient botClient, IConversationAgent conversationalAgent, IPictureProcessor picProcessAgent, ILogger logger, CancellationToken cancellationToken)
{

    if (msg.Photo == null)
    {
        await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.IncorrectInputHandle());
        await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.AskForPassport());
        return;
    } // TODO : Change errors to more conditional depends on stage.

    await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.AskForWaitWhileProcessing());


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

        try
        {
            passportData = await picProcessAgent.ProcessPassportPicture(fileStream.GetBuffer());
        }
        catch (PictureProcessException e)
        {
            logger.LogError(e.Message);
            await botClient.SendTextMessageAsync(chatData.ChatId, e.Message);
            await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.AskForPassportAgain());
            // TODO : possibly develop 1 more method for IConversationalAgent in case if photo was rejected by mindee
            return;
        }
    }
    chatData.PassportData = passportData;

    await AskGeneric(chatData.ChatId, botClient, await conversationalAgent.AskForPassportApprove(passportData));

    chatData.Stage = ProcessStage.WaitingForPassportDataApprove;
}
static async Task ProcessLicensePlatePhoto(ChatData chatData, Message msg, ITelegramBotClient botClient, IConversationAgent conversationalAgent, IPictureProcessor picProcessAgent, ILogger logger, CancellationToken cancellationToken)
{
    if (msg.Photo == null)
    {
        await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.IncorrectInputHandle());
        await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.AskForLicensePlateId());
        return;
    } // TODO : Change errors to more conditional depends on stage.

    await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.AskForWaitWhileProcessing());

    var photoId = msg.Photo.Last().FileId;

    var fileInfo = await botClient.GetFileAsync(photoId);
    var path = fileInfo.FilePath;

    LicensePlateData licensePlateData;
    using (var fileStream = new MemoryStream())
    {
        await botClient.DownloadFileAsync(
            filePath: path,
            destination: fileStream,
            cancellationToken: cancellationToken);

        try
        {
            licensePlateData = await picProcessAgent.ProcessLicensePlatePicture(fileStream.GetBuffer());
        }
        catch (PictureProcessException e)
        {
            logger.LogError(e.Message);
            await botClient.SendTextMessageAsync(chatData.ChatId, e.Message);
            await botClient.SendTextMessageAsync(chatData.ChatId, await conversationalAgent.AskForLicensePlateIdAgain());
            // TODO : possibly develop 1 more method for IConversationalAgent in case if photo was rejected by mindee
            return;
        }
    }
    chatData.VenicleIdData = licensePlateData;

    await AskGeneric(chatData.ChatId, botClient, await conversationalAgent.AskForLicensePlateIdApprove(licensePlateData));

    chatData.Stage = ProcessStage.WaitingForLicensePlateDataApprove;
}

static async Task ReAskForPassportDataApprove(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    var errorText = await conversationalAgent.IncorrectInputHandle();
    var reAskText = await conversationalAgent.AskForPassportApprove(chatData.PassportData);
    await ReAskGeneric(chatData.ChatId, botClient, errorText, reAskText);
}
static async Task ReAskForLicensePlateDataApprove(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    var errorText = await conversationalAgent.IncorrectInputHandle();
    var reAskText = await conversationalAgent.AskForLicensePlateIdApprove(chatData.VenicleIdData);
    await ReAskGeneric(chatData.ChatId, botClient, errorText, reAskText);
}
static async Task ReAskForPriceApprove(ChatData chatData, ITelegramBotClient botClient, IConversationAgent conversationalAgent)
{
    var errorText = await conversationalAgent.IncorrectInputHandle();
    var reAskText = await conversationalAgent.PriceAnnouncement(100);
    await ReAskGeneric(chatData.ChatId, botClient, errorText, reAskText);
}

static async Task ReAskGeneric(long chatId, ITelegramBotClient botClient, string errorText, string reAskText)
{
    await botClient.SendTextMessageAsync(chatId, errorText);
    await AskGeneric(chatId, botClient, reAskText);
}

static async Task AskGeneric(long chatId, ITelegramBotClient botClient, string askText)
{
    var acceptButton = InlineKeyboardButton.WithCallbackData("Yes", InlineReplies.Confirm.ToString());
    var rejectButton = InlineKeyboardButton.WithCallbackData("No", InlineReplies.Unconfirm.ToString());
    await botClient.SendTextMessageAsync(chatId, askText, replyMarkup: new InlineKeyboardMarkup([acceptButton, rejectButton]));
}