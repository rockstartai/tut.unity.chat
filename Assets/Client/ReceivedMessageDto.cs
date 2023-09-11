using System;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public class ReceivedMessageDto
	{
		public string username;
		public string text;
		public long timestamp;


		public DateTime TimestampToDateUtc()
		{
			return new(timestamp * TimeSpan.TicksPerSecond * 1000, DateTimeKind.Utc);
		}
	}
}
