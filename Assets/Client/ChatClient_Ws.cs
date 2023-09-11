using NativeWebSocket;
using Rockstart.Unity.Tut.Chat.ScrollView;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public class ChatClient_Ws : MonoBehaviour, IChatClient
	{
		[SerializeField] string _endpoint;

		WebSocket _websocket;
		IMessageHandler _messageHandler;

		bool _isInitialized;


		async Task IChatClient.InitAsync()
		{
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

				var msg = JsonUtility.FromJson<ReceivedMessageDto>(message);
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

		void IChatClient.SetMessageHandler(IMessageHandler messageHandler)
		{
			_messageHandler = messageHandler;
		}

		async Task IChatClient.SendAsync(SentMessageDto msg)
		{
			if (_websocket.State != WebSocketState.Open)
				return;

			var json = JsonUtility.ToJson(msg);
			await _websocket.SendText(json);
		}

		async void OnApplicationQuit()
		{
			await _websocket.Close();
			_websocket = null;
			_isInitialized = false;
		}
	}
}
