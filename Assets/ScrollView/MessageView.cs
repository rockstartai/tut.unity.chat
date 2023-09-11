using Rockstart.Unity.Tut.Chat.Client;
using UnityEngine;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class MessageView: MonoBehaviour
	{
		public TMPro.TextMeshProUGUI username;
		public TMPro.TextMeshProUGUI text;
		public TMPro.TextMeshProUGUI date;


		public void UpdateViews(ReceivedMessageDto msg)
		{
			username.text = msg.username;
			text.text = msg.text;
			var msgDate = msg.TimestampToDateUtc();
			date.text = msgDate.ToShortTimeString() + " " + msgDate.ToShortDateString();
		}
	}
}
