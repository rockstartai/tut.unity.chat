using com.xucian.upm.grabtex;
using Cysharp.Threading.Tasks;
using Rockstart.Unity.Tut.Chat.Data;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Rockstart.Unity.Tut.Chat.ScrollView
{
	public class MessageView: MonoBehaviour
	{
		public TMPro.TextMeshProUGUI username;
		public TMPro.TextMeshProUGUI text;
		public TMPro.TextMeshProUGUI date;
		public LayoutElement imageLayoutElement;
		public RawImage image;
		public LayoutGroup layoutGroup;
		public float maxImageHeight = 500f;


		public void UpdateViews(MessageModel msg)
		{
			username.text = msg.username;

			// Hyperlinking URLs, thanks: https://stackoverflow.com/a/36661544
			var rgxUrls = new Regex("(((http|ftp|https):\\/\\/)?[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:\\/~\\+#]*[\\w\\-\\@?^=%&amp;\\/~\\+#])?)");
			string textWithHyperlinks = rgxUrls.Replace(msg.text, "<link=\"$1\">$1</link>");
			text.text = textWithHyperlinks;

			// Load the image from the first encountered URL
			var match = rgxUrls.Match(msg.text);
			if (match.Success)
			{
				var imgUrl = match.Value;
				var ct = this.GetCancellationTokenOnDestroy();

				LoadImageAsync(imgUrl, ct).Forget();
			}

			var msgDate = msg.TimestampToDateUtc();
			date.text = msgDate.ToShortTimeString() + " " + msgDate.ToShortDateString();
		}

		async UniTask LoadImageAsync(string url, CancellationToken ct)
		{
			try
			{
				await new GrabTex().IntoAsync(url, image, ct);

				if (ct.IsCancellationRequested || !image.texture)
					return;

				imageLayoutElement.gameObject.SetActive(true);
				UpdateImageAspectRatio();
			}
			catch (UnityWebRequestException) { /* Network-related stuff isn't a concern */}
			catch (Exception e)
			{
				Debug.Log(e);
			}
		}

		void UpdateImageAspectRatio()
		{
			var tex = image.texture;
			float aspectRatio = (float)tex.width / tex.height;

			var availWidth = (layoutGroup.transform as RectTransform).rect.width;
			availWidth -= layoutGroup.padding.horizontal;
			var height = availWidth / aspectRatio;
			height = Mathf.Min(maxImageHeight, height);
			imageLayoutElement.preferredHeight = height;
			imageLayoutElement.preferredWidth = height * aspectRatio;
		}
	}
}
