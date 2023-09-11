using Rockstart.Unity.Tut.Chat.Client;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class MessageView: MonoBehaviour
	{
		public Text username;
		public Text text;
		public Text date;


		public void UpdateViews(ReceivedMessageDto msg)
		{
			username.text = msg.username;
			text.text = msg.text;
			var msgDate = msg.TimestampToDateUtc();
			date.text = msgDate.ToShortTimeString() + " " + msgDate.ToShortDateString();
		}
	}
}
