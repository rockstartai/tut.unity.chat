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
			var timestampSec = timestamp / 1000f;
			var timestampTicks = timestampSec * TimeSpan.TicksPerSecond;
			var ticks = (long)(timestampTicks + .5f);  // rounding to long int
			return new(ticks, DateTimeKind.Utc);
		}
	}
}
