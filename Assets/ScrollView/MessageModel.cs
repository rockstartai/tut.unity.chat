using System;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class MessageModel
	{
		public string username;
		public string text;
		public long timestamp;

		const string SEPARATOR = "|:#";


		public MessageModel()
		{

		}

		public MessageModel(string msgData)
		{
			var split = msgData.Split(SEPARATOR);
			//(username, text, timestamp) = (split[0], split[1], StrToDateUtc(split[2]));
			(username, text, timestamp) = (split[0], split[1], long.Parse(split[2]));
		}


		//public override string ToString()
		//{
		//	return UtcDateToStr(date) + SEPARATOR + username + SEPARATOR + text;
		//}

		public DateTime TimestampToDateUtc()
		{
			return new(timestamp * TimeSpan.TicksPerSecond, DateTimeKind.Utc);
		}

		//static string UtcDateToStr(DateTime date)
		//{
		//	return new DateTimeOffset(date).ToUnixTimeSeconds().ToString();
		//}

		//static DateTime StrToDateUtc(string dateStr)
		//{
		//	long unixTimestamp = long.Parse(dateStr);
		//	return new(unixTimestamp * TimeSpan.TicksPerSecond, DateTimeKind.Utc);
		//}
	}
}
