using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botId = Environment.GetEnvironmentVariable("BotToken");

var serviceProvider = ConfigureServices();

var botClient = new TelegramBotClient(botId);
using CancellationTokenSource cts = new();
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
    return services.BuildServiceProvider();
}
static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var msg = update.Message;
    if (msg == null)
        return;

    var chatId = update.Message.Chat.Id;

    await botClient.SendTextMessageAsync(chatId, "echo");
    return;
}

static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
    None,
    WaitingForPassportPhoto,
    WaitingForPassportDataApprove,
    WaitingForVenichleIdPhoto,
    WaitingForVenichleIdDataApprove,
    WaitingForPriceConfirmation,
}