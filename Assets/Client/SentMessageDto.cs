using UnityEngine;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class SentMessageDto
	{
		public string username;
		public string text;


		public WWWForm ToWwwForm()
		{
			var form = new WWWForm();
			form.AddField("username", username);
			form.AddField("text", text);

			return form;
		}
	}
}
