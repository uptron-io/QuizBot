using System;
namespace BotQuiz.Service
{
	public static class Utils
	{
        public static TimeZoneInfo GetCurrentTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time");
        }

        public static DateTime GetCurrentDateTime()
		{
            TimeZoneInfo tzi = GetCurrentTimeZone();
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);
        }
	}
}

