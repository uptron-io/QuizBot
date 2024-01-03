using System;
namespace QuizBot
{
    public record AnswerModel(string QuizName, string Name, string Answer, string ChatId, DateTime CreatedAt);
    public record ChatData(string ChatId, string Name);
}

