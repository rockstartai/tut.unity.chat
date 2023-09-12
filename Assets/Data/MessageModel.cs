using System;

namespace Rockstart.Unity.Tut.Chat.Data
{
	public class MessageModel
	{
		public string username;
		public string text;
		public long timestamp;


		public DateTime TimestampToDateUtc()
		{
			return DateTime.UnixEpoch.AddMilliseconds(timestamp);
		}
	}
}
