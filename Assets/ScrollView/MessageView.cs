using Cysharp.Threading.Tasks;
using Rockstart.Unity.Tut.Chat.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
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
			string textWithHyperlinks = rgxUrls.Replace(msg.text, "<link=\"$1\">$1</link>");
			text.text = textWithHyperlinks;

			var rgxImgUrl = new Regex("@(https?:)?//?[^'\"<>]+?\\.(jpg|jpeg|gif|png)@");
			var match = rgxImgUrl.Match(msg.text);
			image.gameObject.SetActive(match.Success);
			if (match.Success)
			{
				var imgUrl = match.Value;
				LoadImageAsync(imgUrl).Forget();
			}

			var msgDate = msg.TimestampToDateUtc();
			date.text = msgDate.ToShortTimeString() + " " + msgDate.ToShortDateString();
		}

		async UniTask LoadImageAsync(string url)
		{
			using (var www = UnityWebRequestTexture.GetTexture(url))
			{
				await www.SendWebRequest();

				if (www.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError("Error: " + www.error);
					return;
				}

				var loadedTexture = DownloadHandlerTexture.GetContent(www);
				image.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
				//image.SetNativeSize();
			}

		}
	}
}
