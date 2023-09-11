using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class MessageView: MonoBehaviour
	{
		public Text from;
		public Text msg;
		public Text date;


		public void UpdateViews(MessageModel model)
		{
			from.text = from.text;
			msg.text = msg.text;
			date.text = model.date.ToShortTimeString() + " " + model.date.ToShortDateString();
		}
	}
}
