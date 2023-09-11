using NativeWebSocket;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public class ChatClient : MonoBehaviour
	{
		string _endpoint = "ws://jittery-duck-blazer.cyclic.app:3000";

		WebSocket _websocket;
		IMessageHandler _messageHandler;


		public async Task InitAsync()
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
				_messageHandler.HandleMessage(message);
			};

			await _websocket.Connect();
		}

		void Update()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			_websocket.DispatchMessageQueue();
#endif
		}

		public void SetMessageHandler(IMessageHandler messageHandler)
		{
			_messageHandler = messageHandler;
		}

		public async Task SendAsync(string msgData)
		{
			if (_websocket.State != WebSocketState.Open)
				return;

			await _websocket.SendText(msgData);
		}

		async void OnApplicationQuit()
		{
			await _websocket.Close();
		}
	}
}
