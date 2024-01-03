using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace QuizBot
{
    public interface IGoogleSheetIntegration
    {
        QuizSettings GetQuizSettings(string sheetName);
        string GetLastChatId();
        List<QuizItem> GetQuizItems(string sheetName);
        List<string> GetChatId();
        List<ChatData> GetChatData();
        void SaveData(string googleRange, IList<IList<object>> values);
        void Dispose();
    }

    public class GoogleSheetIntegration : IGoogleSheetIntegration
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "Quiz Bot";
        static readonly string SpreadsheetId = "1KrRDQGICbUzzXxPBN-1n9kOhjXC6uLJpHquJ_GkeO4Q";
        static SheetsService service;


        public GoogleSheetIntegration()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("client-secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public QuizSettings GetQuizSettings(string sheetName)
        {
            var introduction = GetString(sheetName, "B1:F1");
            var endText = GetString(sheetName, "B2:F2");

            return new QuizSettings(introduction, endText);
        }

        public string GetLastChatId()
        {
            var table = GetValue("ChatId", "A2:A20");

            if (table != null && table.Any())
            {
                return (string)table.Last()[0];
            }

            return string.Empty;
        }

        public List<QuizItem> GetQuizItems(string sheetName)
        {
            var quizItems = new List<QuizItem>();
            var table = GetValue(sheetName, "A8:I108");

            if (table == null) return quizItems;

            foreach (var record in table)
            {
                string name = (string)record[0];

                var items = record.Skip(1) // Skip the name field.
                                  .Select(r => r.ToString()) // Convert each record to string.
                                  .Where(s => !string.IsNullOrEmpty(s)) // Filter out empty strings.
                                  .ToList();

                quizItems.Add(new QuizItem(name, items));
            }

            return quizItems;
        }

        public List<string> GetChatId()
        {
            var table = this.GetValue("ChatId", "A2:A501");
            if (table == null)
            {
                return new List<string>();
            }

            var chatIdList = table
                .Select(record => (string)record[0])
                .ToList();

            return chatIdList;
        }

        public List<ChatData> GetChatData()
        {
            var table = this.GetValue("ChatId", "A2:B501");
            if (table == null)
            {
                return new List<ChatData>();
            }

            var chatDataList = table
                .Where(record => !string.IsNullOrEmpty(record[0] as string))
                .Select(record => new ChatData((string)record[0], (string)record[1]))
                .ToList();

            return chatDataList;
        }

        public void SaveData(string googleRange, IList<IList<object>> values)
        {
            var request = service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId: SpreadsheetId, range: googleRange);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            request.Execute();
        }

        public void Dispose()
        {
            service.Dispose();
        }

        private string GetString(string sheet, string range)
        {
            var tmp = GetValue(sheet, range);
            return (string)tmp?[0]?[0] ?? string.Empty;
        }

        private IList<IList<object>>? GetValue(string sheet, string range)
        {
            IList<IList<object>>? data = null;

            for (int attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    var googleRange = $"{sheet}!{range}";
                    var request = service.Spreadsheets.Values.Get(SpreadsheetId, googleRange);
                    var response = request.Execute();
                    data = response.Values;
                    break;
                }
                catch { }
                Thread.Sleep(1000);
            }

            return data;
        }
    }

}

