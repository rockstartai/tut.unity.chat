using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Rockstart.Unity.Tut.Chat.Data;
using System;
using UnityEngine;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public class ChatClient_Ws : MonoBehaviour, IChatClient
	{
		[SerializeField] string _endpoint;

		WebSocket _websocket;
		IMessageHandler _messageHandler;

		bool _isInitialized;


		async UniTask IChatClient.InitAsync(IMessageHandler messageHandler)
		{
			_messageHandler = messageHandler;
			_websocket = new WebSocket(_endpoint);

			_websocket.OnOpen += () =>
			{
				Debug.Log("Connection open!");
			};

			_websocket.OnError += (e) =>
			{
				Debug.Log("Error! " + e);
			};

			_websocket.OnClose += (e) =>
			{
				Debug.Log("Connection closed! " + e);
			};

			_websocket.OnMessage += (bytes) =>
			{
				// Assuming server sends plain text messages
				var message = System.Text.Encoding.UTF8.GetString(bytes);
				Debug.Log("Received! " + message);

				var msg = JsonUtility.FromJson<MessageModel>(message);
				_messageHandler.HandleMessage(msg);
			};

			await _websocket.Connect();

			_isInitialized = true;
		}

		void Update()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			if (_isInitialized)
				_websocket.DispatchMessageQueue();
#endif
		}

		async UniTask IChatClient.SendAsync(MessageModel msg)
		{
			if (_websocket.State != WebSocketState.Open)
				throw new Exception("not open");

			var json = JsonUtility.ToJson(msg);
			await _websocket.SendText(json);
		}

		async void OnApplicationQuit()
		{
			await _websocket.Close();
			_websocket = null;
			_isInitialized = false;
		}

		public void Dispose()
		{
		}
	}
}
