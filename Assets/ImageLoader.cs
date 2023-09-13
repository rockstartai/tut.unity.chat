using Cysharp.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebP;

namespace Rockstart.Unity.Tut.Chat
{
	public class ImageLoader
	{
		public async UniTask LoadImageAsync(string url, RawImage into, CancellationToken cancellation)
		{
			try
			{
				var contentType = await GetContentTypeAsync(url);
				string imageContentType;
				string imageUrl;

				if (contentType.StartsWith("image/"))
				{
					imageContentType = contentType;
					imageUrl = url;
				}
				else if (contentType.StartsWith("text/html"))
				{
					imageUrl = await FindImageInHtmlAsync(url, cancellation);

					if (imageUrl == null)
						return;

					if (cancellation.IsCancellationRequested)
						return;

					imageContentType = await GetContentTypeAsync(imageUrl);
				}
				else
				{
					return;
				}

				if (cancellation.IsCancellationRequested)
					return;

				var tex = await DownloadImageAsync(imageUrl, imageContentType, cancellation);
				if (!tex) 
					return;

				if (cancellation.IsCancellationRequested)
				{
					UnityEngine.Object.Destroy(tex);
					return;
				}

				if (!into)
					return;

				//into.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
				into.texture = tex;
				into.gameObject.SetActive(true);
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}
		}

		async UniTask<Texture2D> DownloadImageAsync(string url, string contentType, CancellationToken cancellation)
		{
			try
			{
				contentType ??= await GetContentTypeAsync(url);
				Texture2D tex;

				if (contentType.StartsWith("image/webp"))
					tex = await LoadWebpTextureAsync(url, cancellation);
				else
					tex = await LoadRegularTextureAsync(url, cancellation);

				if (!tex)
					return null;

				if (cancellation.IsCancellationRequested)
				{
					UnityEngine.Object.Destroy(tex);
					return null;
				}

				return tex;
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}

			return null;
		}

		async UniTask<string> FindImageInHtmlAsync(string url, CancellationToken cancellation)
		{
			using (var req = UnityWebRequest.Get(url))
			{
				SetRequestHeaders(req);
				await req.SendWebRequest();

				if (req.result != UnityWebRequest.Result.Success)
				{
					Debug.Log("Error: " + req.error);
					return null;
				}

				string pageContent = req.downloadHandler.text;
				return ParseHtmlForOGImage(pageContent);
			}
		}

		string ParseHtmlForOGImage(string htmlContent)
		{
			var match = Regex.Match(htmlContent, "<meta property=\"og:image\" content=\"(.*?)\"");
			if (!match.Success)
				return null;

			var ogImageUrl = match.Groups[1].Value;
			return ogImageUrl;
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
				SetRequestHeaders(req);
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
				throw new AggregateException(
					e, 
					new Exception($"Download handler err: {req.downloadHandler.error}")
				);
			}

			if (cancellation.IsCancellationRequested)
				return;

			if (req.result != UnityWebRequest.Result.Success)
				Debug.Log("Error: " + req.error);
		}

		async UniTask<string> GetContentTypeAsync(string url)
		{
			using (var req = UnityWebRequest.Head(url))
			{
				SetRequestHeaders(req);
				await req.SendWebRequest();
				return req.GetResponseHeader("Content-Type");
			}
		}

		void SetRequestHeaders(UnityWebRequest req)
		{
			req.SetRequestHeader("User-Agent", "rockstarai.tut.unity.chat");  // some websites return "403 forbidden" if no User-Agent header
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
