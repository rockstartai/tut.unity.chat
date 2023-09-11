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
		[SerializeField] CanvasGroup _nickInputPanel;
		[SerializeField] MessagesController _msgController;
		[SerializeField] TMPro.TMP_InputField _msgInput;
		[SerializeField] Button _sendButton;

		IChatClient _client;
		string _nick;


		void Start()
		{
			_client = GetComponent<IChatClient>();
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
			_nickInputPanel.interactable = false;
			await _client.InitAsync();
			_nickInputPanel.gameObject.SetActive(false);
			_nick = _nickInput.text;
			_msgController.Init(_nick);
		}

		void OnSendClicked()
		{
			if (_msgInput.text == string.Empty)
				return;

			var msg = new SentMessageDto
			{
				username = _nick,
				text = _msgInput.text,
			};

			_ = SendAsync(msg);
			_msgInput.text = "";
		}

		async Task SendAsync(SentMessageDto msg)
		{
			await _client.SendAsync(msg);
		}

		void IMessageHandler.HandleMessage(ReceivedMessageDto msg)
		{
			_msgController.InsertMessage(msg);
		}
	}
}
