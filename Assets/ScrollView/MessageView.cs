using Cysharp.Threading.Tasks;
using Rockstart.Unity.Tut.Chat.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class MessageView: MonoBehaviour
	{
		public TMPro.TextMeshProUGUI username;
		public TMPro.TextMeshProUGUI text;
		public TMPro.TextMeshProUGUI date;
		public Image image;


		public void UpdateViews(MessageModel msg)
		{
			username.text = msg.username;

			// Hyperlinking URLs, thanks: https://stackoverflow.com/a/36661544
			var rgxUrls = new Regex("(((http|ftp|https):\\/\\/)?[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:\\/~\\+#]*[\\w\\-\\@?^=%&amp;\\/~\\+#])?)");
			string textWithHyperlinks = rgxUrls.Replace(msg.text, "<link=\"$1\"> $1 </link>");
			text.text = textWithHyperlinks;

			// Load the image from the first encountered URL
			var match = rgxUrls.Match(msg.text);
			if (match.Success)
			{
				var imgUrl = match.Value;
				var cancelToken = this.GetCancellationTokenOnDestroy();
				new ImageLoader().LoadImageAsync(imgUrl, image, cancelToken).Forget();
			}

			var msgDate = msg.TimestampToDateUtc();
			date.text = msgDate.ToShortTimeString() + " " + msgDate.ToShortDateString();
		}
	}
}
