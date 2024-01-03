namespace QuizBot;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton<IGoogleSheetIntegration, GoogleSheetIntegration>();
            })
            .Build();

        host.Run();
    }
}
