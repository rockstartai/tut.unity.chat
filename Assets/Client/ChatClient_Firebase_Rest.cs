using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using com.forbiddenbyte.tut.unity.chat.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.forbiddenbyte.tut.unity.chat.Client
{
	public class ChatClient_Firebase_Rest : MonoBehaviour, IChatClient
	{
		[SerializeField] string _endpoint;

		// Use a reliable timestamp provider instead of user's device
		const string TIMESTAMP_URL = "https://worldtimeapi.org/api/timezone/Etc/UTC";

		string _chatDbUrl;
		IMessageHandler _messageHandler;
		bool _isInitialized;
		long _lastHandledTimestamp = 0;


		UniTask IChatClient.InitAsync(IMessageHandler messageHandler)
		{
			_messageHandler = messageHandler;
			_chatDbUrl = $"{_endpoint}/messages.json";
			_isInitialized = true;

			StartUpdateLoopAsync().Forget();

			return UniTask.CompletedTask;
		}

		async UniTask IChatClient.SendAsync(MessageModel msg)
		{
			var timestamp = await GetTimestampAsync();
			if (timestamp == null)
				throw new Exception("Timestamp couldn't be retrieved");

			msg.timestamp = timestamp.Value;
			string jsonData = JsonConvert.SerializeObject(msg);

			//using (var req = UnityWebRequest.Post(_chatDbUrl, jsonData))
			using (var req = UnityWebRequest.Put(_chatDbUrl, jsonData))
			{
				req.method = UnityWebRequest.kHttpVerbPOST; // Use POST to add new entries, UnityWebRequest.Post doesn't work for some reason
				req.SetRequestHeader("Content-Type", "application/json");
				await req.SendWebRequest();

				if (req.result == UnityWebRequest.Result.ConnectionError)
					throw new Exception(req.error);
			}
		}

		async UniTask StartUpdateLoopAsync()
		{
			try
			{
				while (_isInitialized)
				{
					try { await HandleNewMessagesAsync(); } catch (Exception e) { Debug.LogException(e); }
					await UniTask.Delay(500);
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		async UniTask<string> HandleNewMessagesAsync()
		{
			long timestamp = _lastHandledTimestamp;
			string query = $"orderBy=\"timestamp\"&startAt={timestamp}";
			using (var req = UnityWebRequest.Get(_chatDbUrl + "?" + query))
			{
				await req.SendWebRequest();

				if (req.result != UnityWebRequest.Result.Success)
					return req.error;

				var text = req.downloadHandler.text;
				if (string.IsNullOrEmpty(text))
					return null;

				var messagesDict = JsonConvert.DeserializeObject<Dictionary<string, MessageModel>>(text);
				var messages = new List<MessageModel>(messagesDict.Values);
				// Firebase docs state that the returned items aren't sorted, even though they correctly obey orderBy and startAt
				messages.Sort((a, b) => a.timestamp - b.timestamp < 0 ? -1 : 1);

				// Handle new messages since last handled one
				messages = messages.FindAll(msg => msg.timestamp > _lastHandledTimestamp);

				if (messages.Count == 0)
					return null;

				_messageHandler?.HandleMessages(messages);
				_lastHandledTimestamp = messages[^1].timestamp;
			}

			return null;
		}

		async UniTask<long?> GetTimestampAsync()
		{
			using (var www = UnityWebRequest.Get(TIMESTAMP_URL))
			{
				await www.SendWebRequest();
				if (www.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.LogError("Error: " + www.error);
					return null;
				}

				string jsonResponse = www.downloadHandler.text;
				var jsonObject = JObject.Parse(jsonResponse);
				long timestamp = (long)jsonObject["unixtime"] * 1000;

				return timestamp;
			}
		}

		public void Dispose()
		{
			_isInitialized = false;
		}
	}
}
