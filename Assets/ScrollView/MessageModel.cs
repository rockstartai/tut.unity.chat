using System;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class MessageModel
	{
		public DateTime date;
		public string sender;
		public string msg;

		const string SEPARATOR = "|:#";


		public MessageModel(string msgData)
		{
			var split = msgData.Split(SEPARATOR);
			(sender, msg, date) = (split[0], split[1], StrToDateUtc(split[2]));
		}


		public override string ToString()
		{
			return UtcDateToStr(date) + SEPARATOR + sender + SEPARATOR + msg;
		}

		static string UtcDateToStr(DateTime date)
		{
			return new DateTimeOffset(date).ToUnixTimeSeconds().ToString();
		}

		static DateTime StrToDateUtc(string dateStr)
		{
			long unixTimestamp = long.Parse(dateStr);
			return new(unixTimestamp * TimeSpan.TicksPerSecond, DateTimeKind.Utc);
		}
	}
}
