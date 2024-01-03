using BotQuiz.Service;
using Cronos;
using Sgbj.Cron;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QuizBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IGoogleSheetIntegration _googleSheetIntegration;

    private int minutesToWait = 60;

    public Worker(ILogger<Worker> logger, IGoogleSheetIntegration googleSheetIntegration)
    {
        _logger = logger;
        _googleSheetIntegration = googleSheetIntegration;
    }

    public Dictionary<string, string> conversations = new Dictionary<string, string>();
    public Dictionary<string, QuizPollSendItem> pollSendItems = new Dictionary<string, QuizPollSendItem>();
    public Dictionary<string, ConversationMode> conversationMode = new Dictionary<string, ConversationMode>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start worker");

        TimeZoneInfo tzi = Utils.GetCurrentTimeZone();

        var botClient = new TelegramBotClient("6659784166:AAECAdZVxzXdqT9-EibU-MiTppZYFKTe3z4"); //Your token from Bot Father

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );


        CreateWorker(botClient, tzi, stoppingToken);
    }

    protected async Task CreateWorker(TelegramBotClient botClient, TimeZoneInfo tzi, CancellationToken stoppingToken)
    {
        using var timer = new CronTimer(CronExpression.Parse("*/3 * * * *", CronFormat.Standard), tzi);
        var sheetName = "Checklist";

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Start send questions");

            List<QuizResultItem> result = new List<QuizResultItem>();

            var settings = _googleSheetIntegration.GetQuizSettings(sheetName);

            _logger.LogInformation(settings.Introduction);

            string chatId = _googleSheetIntegration.GetLastChatId();

            _logger.LogInformation(chatId);

            if (conversationMode.ContainsKey(chatId))
            {
                conversationMode[chatId] = ConversationMode.Checklist;
            }
            else
            {
                conversationMode.Add(chatId, ConversationMode.Checklist);
            }

            //Introduction
            await SendTextMessage(botClient, chatId, settings.Introduction);

            //Send questions
            var quizItems = _googleSheetIntegration.GetQuizItems(sheetName);

            bool noAnswer = false;

            foreach (var quizItem in quizItems)
            {
                _logger.LogInformation($"Send question: {quizItem.Name}");

                if (conversations.ContainsKey(chatId))
                {
                    conversations[chatId] = string.Empty;
                }
                else
                {
                    conversations.Add(chatId, string.Empty);
                }

                await SendListMessage(botClient, chatId, quizItem.Name, quizItem.Items);

                var startTime = DateTime.UtcNow;
                while (conversations[chatId] == string.Empty)
                {
                    await Task.Delay(1000);

                    if (startTime.AddMinutes(minutesToWait) < DateTime.UtcNow)
                    {
                        _logger.LogInformation($"Close session by timer, no answers from {chatId}");

                        conversations.Remove(chatId);

                        //Long wait
                        await SendTextMessage(botClient, chatId, "");
                        noAnswer = true;
                        break;
                    }
                }

                if (noAnswer == true)
                {
                    break;
                }

                result.Add(new QuizResultItem(sheetName, quizItem.Name, conversations[chatId], chatId, Utils.GetCurrentDateTime()));
            }

            conversationMode[chatId] = ConversationMode.Wait;

            if (noAnswer == false)
            {
                _logger.LogInformation($"Answered on all questions: {chatId}");

                //Send Final Words
                await SendTextMessage(botClient, chatId, settings.FinalWords);

                //Save answers
                _googleSheetIntegration.SaveData("Answers!A:E", ConvertToList(result));
            }
        }
    }

    private IList<IList<object>> ConvertToList(List<QuizResultItem> items)
    {
        return items.Select(item => new List<object>
        {
            item.Name,
            item.Answer,
            item.ChatId,
            item.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")
        }).Cast<IList<object>>().ToList();
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        string messageText = string.Empty;
        string chatId = string.Empty;

        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message != null && update.Message!.Text != null)
        {
            messageText = update.Message.Text;
            chatId = update.Message.Chat.Id.ToString();
        }

        if (update.PollAnswer != null)
        {
            chatId = update.PollAnswer.User.Id.ToString();

            if (pollSendItems.ContainsKey(update.PollAnswer.PollId))
            {
                var pollAnswerIndex = update.PollAnswer.OptionIds[0];
                messageText = pollSendItems[update.PollAnswer.PollId].Items[pollAnswerIndex];

                pollSendItems.Remove(update.PollAnswer.PollId);
            }
        }

        if (string.IsNullOrEmpty(messageText) || string.IsNullOrEmpty(chatId))
            return;

        _logger.LogInformation($"Received a '{messageText}' message in chat {chatId}.");

        if (conversationMode.ContainsKey(chatId) == false)
        {
            conversationMode.Add(chatId, ConversationMode.Wait);
        }

        if (conversationMode.ContainsKey(chatId) && conversationMode[chatId] == ConversationMode.Checklist)
        {
            conversations[chatId] = messageText;
        }
        else
        {
            if (conversationMode[chatId] == ConversationMode.Wait)
            {
                var chatIdList = _googleSheetIntegration.GetChatId();

                if (chatIdList.Contains(chatId))
                {
                    conversationMode[chatId] = ConversationMode.Wait;

                    _logger.LogInformation($"User {chatId} had been registered in the system before");

                    //send Hello message.
                    await SendTextMessage(botClient: botClient, chatId: chatId, description: "You are already registered in the system!");
                }
                else
                {
                    conversationMode[chatId] = ConversationMode.NewUser;

                    _logger.LogInformation($"New user {chatId}");

                    //new user
                    await SendTextMessage(botClient: botClient, chatId: chatId, description: "Hi! You must go through the registration procedure. What is your name?");
                }
            }
            else if (conversationMode.ContainsKey(chatId) == true && conversationMode[chatId] == ConversationMode.NewUser)
            {
                //registration process
                IList<IList<object>> data = new List<IList<object>>();
                IList<object> dataItem = new List<object>();
                dataItem.Add(chatId);
                dataItem.Add(messageText);
                data.Add(dataItem);

                _googleSheetIntegration.SaveData("ChatId!A:B", data);

                conversationMode[chatId] = ConversationMode.Wait;

                await SendTextMessage(botClient: botClient, chatId: chatId, description: "Thank you! You have successfully registered in the system!");

                _logger.LogInformation($"New user {chatId} had been registered");

            }
            else
            {
                _logger.LogInformation($"User {chatId} had been registered in the system before");

                //send Hello message.
                await SendTextMessage(botClient: botClient, chatId: chatId, description: "You are already registered in the system!");
            }
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

        _logger.LogError(ErrorMessage);

        return Task.CompletedTask;
    }

    private async Task SendTextMessage(ITelegramBotClient botClient, string chatId, string description)
    {
        _logger.LogInformation($"Send message: {chatId}/{description}");

        await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: description,
                disableNotification: false);
    }

    private async Task SendListMessage(ITelegramBotClient botClient, string chatId, string question, List<string> options)
    {
        await SendTextMessage(botClient, chatId, question);

        _logger.LogInformation($"Send poll: {chatId}/{question}");

        var message = await botClient.SendPollAsync(
                chatId: chatId,
                question: "Answer options:",
                options: options,
                isAnonymous: false,
                type: PollType.Regular,
                allowSendingWithoutReply: false,
                disableNotification: false);

        if (message?.Poll != null)
        {
            var pollId = message.Poll.Id;

            if (pollSendItems.ContainsKey(pollId))
            {
                pollSendItems[pollId] = new QuizPollSendItem(chatId, pollId, options);
            }
            else
            {
                pollSendItems.Add(pollId, new QuizPollSendItem(chatId, pollId, options));
            }
        }
    }
}


