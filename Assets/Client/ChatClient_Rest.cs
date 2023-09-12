//using Cysharp.Threading.Tasks;
//using Newtonsoft.Json;
//using Rockstart.Unity.Tut.Chat.Data;
//using Rockstart.Unity.Tut.Chat.ScrollView;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace Rockstart.Unity.Tut.Chat.Client
//{
//	public class ChatClient_Rest : MonoBehaviour, IChatClient
//	{
//		[SerializeField] string _endpoint;

//		IMessageHandler _messageHandler;
//		bool _isInitialized;
//		Coroutine _retrieveMessagesCoroutine;
//		long _lastReceivedTimestamp = 0;
//		float _lastUpdate = 0f;


//		UniTask IChatClient.InitAsync(IMessageHandler messageHandler)
//		{
//			_messageHandler = messageHandler;
//			_isInitialized = true;
//			return UniTask.CompletedTask;
//		}

//		void Update()
//		{
//			if (!_isInitialized)
//				return;

//			if (Time.time - _lastUpdate > 1f)
//			{
//				_lastUpdate = Time.time;
//				RetrieveMessages();
//			}
//		}

//		void OnDisable()
//		{
//			if (_retrieveMessagesCoroutine != null)
//			{
//				StopCoroutine(_retrieveMessagesCoroutine);
//				_retrieveMessagesCoroutine = null;
//			}
//		}

//		async UniTask<string> IChatClient.SendAsync(MessageModel msg)
//		{
//			bool done = false;
//			StartCoroutine(SendCoroutine(msg, () =>
//			{
//				done = true;
//			}));

//			while (!done)
//				await UniTask.Delay(100);

//			return null;
//		}

//		IEnumerator SendCoroutine(MessageModel msg, Action onDone)
//		{
//			Debug.Log("Sending");
//			using (var www = UnityWebRequest.Post(_endpoint + "/message", form))
//			{
//				www.timeout = 5;
//				yield return www.SendWebRequest();

//				if (www.result == UnityWebRequest.Result.Success)
//				{
//					Debug.Log("Message sent successfully.");
//				}
//				else
//				{
//					Debug.LogError("Error sending message: " + www.error);
//				}
//			}
//			onDone?.Invoke();
//		}

//		void RetrieveMessages()
//		{
//			if (_retrieveMessagesCoroutine != null)
//				return;

//			_retrieveMessagesCoroutine = StartCoroutine(GetMessagesCoroutine());
//		}

//		IEnumerator GetMessagesCoroutine()
//		{
//			using (var www = UnityWebRequest.Get(_endpoint + "/messages?timestamp=" + _lastReceivedTimestamp))
//			{
//				//var www = UnityWebRequest.Get(_endpoint + "/messages");
//				www.timeout = 5;
//				yield return www.SendWebRequest();

//				if (www.result == UnityWebRequest.Result.Success)
//				{
//					var text = www.downloadHandler.text;
//					if (!string.IsNullOrEmpty(text))
//					{
//						var receivedMessages = JsonConvert.DeserializeObject<List<MessageModel>>(text);

//						for (int i = 0; i < receivedMessages.Count; i++)
//						{
//							// Update the last received timestamp if the new one is bigger.
//							if (receivedMessages[i].timestamp > _lastReceivedTimestamp)
//							{
//								_messageHandler?.HandleMessage(receivedMessages[i]);
//								_lastReceivedTimestamp = receivedMessages[i].timestamp;
//							}
//						}
//					}
//				}
//				else
//				{
//					Debug.LogError("Error retrieving messages: " + www.error);
//				}
//			}
//			_retrieveMessagesCoroutine = null;
//		}

//		void OnApplicationQuit()
//		{
//			_retrieveMessagesCoroutine = null;
//			_isInitialized = false;
//		}

//		public void Dispose()
//		{
//		}
//	}
//}
