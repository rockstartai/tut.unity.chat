using Cysharp.Threading.Tasks;
using Rockstart.Unity.Tut.Chat.Data;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebP;

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

			var match = rgxUrls.Match(msg.text);
			if (match.Success)
			{
				var imgUrl = match.Value;
				var cancelToken = this.GetCancellationTokenOnDestroy();
				CheckForImageAsync(imgUrl, cancelToken).Forget();
			}

			var msgDate = msg.TimestampToDateUtc();
			date.text = msgDate.ToShortTimeString() + " " + msgDate.ToShortDateString();
		}

		async UniTask CheckForImageAsync(string url, CancellationToken cancellation)
		{
			// Especially since webp is handled differently, it makes sense to always check headers
			////var rgxViaImageExtension = new Regex("@(https?:)?//?[^'\"<>]+?\\.(jpg|jpeg|gif|png|webp)@");
			////if (!rgxViaImageExtension.IsMatch(url))
			//{
			//	// Check for headers directly

			//	using (var req = UnityWebRequest.Head(url))
			//	{
			//		await req.SendWebRequest();
			//		var contentType = req.GetResponseHeader("Content-Type");
			//		if (!contentType.StartsWith("image/"))
			//			return;
			//	}

			//	if (cancellationToken.IsCancellationRequested)
			//		return;
			//}

			string contentType;
			using (var req = UnityWebRequest.Head(url))
			{
				await req.SendWebRequest();
				contentType = req.GetResponseHeader("Content-Type");
			}

			if (!contentType.StartsWith("image/"))
				return;

			Texture2D tex;
			if (contentType.StartsWith("image/webp"))
				tex = await LoadWebpTextureAsync(url, cancellation);
			else
				tex = await LoadRegularTextureAsync(url, cancellation);

			if (cancellation.IsCancellationRequested)
				return;

			image.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
			//image.SetNativeSize();
			image.gameObject.SetActive(true);

		}

		async UniTask<Texture2D> LoadRegularTextureAsync(string url, CancellationToken cancellation)
		{
			using (var req = UnityWebRequestTexture.GetTexture(url))
			{
				await SendRequest(req, cancellation);
				if (cancellation.IsCancellationRequested)
					return null;

				return DownloadHandlerTexture.GetContent(req);
			}
		}

		async UniTask<Texture2D> LoadWebpTextureAsync(string url, CancellationToken cancellation)
		{
			using (var req = UnityWebRequest.Get(url))
			{
				await SendRequest(req, cancellation);
				if (cancellation.IsCancellationRequested)
					return null;

				return CreateTextureFromWebpRequest(req);
			}
		}

		async UniTask SendRequest(UnityWebRequest req, CancellationToken cancellation)
		{
			try
			{
				await req.SendWebRequest();
			}
			catch (Exception e)
			{
				Debug.LogError($"Download handler err: {req.downloadHandler.error}");
				throw e;
			}

			if (cancellation.IsCancellationRequested)
				return;

			if (req.result != UnityWebRequest.Result.Success)
				Debug.Log("Error: " + req.error);
		}

		Texture2D CreateTextureFromWebpRequest(UnityWebRequest req)
		{
			var bytes = req.downloadHandler.data;
			var texture = Texture2DExt.CreateTexture2DFromWebP(bytes, lMipmaps: true, lLinear: true, lError: out Error lError);
			if (lError != Error.Success)
			{
				Debug.Log("Webp Load Error : " + lError.ToString());
				return null;
			}

			return texture;
		}
	}
}
