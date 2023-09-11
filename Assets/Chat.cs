using Rockstart.Unity.Tut.Chat.Client;
using Rockstart.Unity.Tut.Chat.ScrollView;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rockstart.Unity.Tut.Chat
{
	public class Chat : MonoBehaviour, IMessageHandler
	{
		[SerializeField] TMPro.TMP_InputField _nickInput;
		[SerializeField] Button _enterButton;
		[SerializeField] GameObject _nickInputPanel;

		[SerializeField] MessagesController _msgController;
		[SerializeField] TMPro.TMP_InputField _msgInput;
		[SerializeField] Button _sendButton;
		[SerializeField] ChatClient _client;


		void Start()
		{
			_client.SetMessageHandler(this);
			_enterButton.onClick.AddListener(OnStartClicked);
			_sendButton.onClick.AddListener(OnSendClicked);
		}

		void OnStartClicked()
		{
			if (_nickInput.text == string.Empty)
				return;

			_ = InitAsync();
		}

		async Task InitAsync()
		{
			_nickInputPanel.gameObject.SetActive(false);
			_msgController.Init(_nickInput.text);
			await _client.InitAsync();
		}

		void OnSendClicked()
		{
			if (_msgInput.text == string.Empty)
				return;

			SendAsync(_msgInput.text);
			_msgInput.text = "";
		}

		async void SendAsync(string msgData)
		{
			await _client.SendAsync(msgData);
		}

		void IMessageHandler.HandleMessage(string message)
		{
			_msgController.InsertMessage(new MessageModel(message));
		}
	}
}
