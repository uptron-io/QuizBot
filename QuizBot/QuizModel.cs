using System;
namespace QuizBot
{
    public record QuizSettings(string Introduction, string FinalWords);

    public record QuizItem(string Name, List<string> Items);

    public record QuizResultItem(string QuizName, string Name, string Answer, string ChatId, DateTime CreatedAt);

    public record QuizPollSendItem(string ChatId, string PollId, List<string> Items);

    public enum ConversationMode
    {
        Wait,
        Checklist,
        NewUser,
    }
}

