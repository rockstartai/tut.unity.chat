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

		bool _isSending;
		IChatClient _client;
		string _nick;


		void Start()
		{
			_client = GetComponent<IChatClient>();
			_client.SetMessageHandler(this);
			_enterButton.onClick.AddListener(OnStartClicked);
			_sendButton.onClick.AddListener(OnSendClicked);

			_nickInput.onSubmit.AddListener(_ => OnStartClicked());
			_msgInput.onSubmit.AddListener(_ => OnSendClicked());
		}

		void Update()
		{
			_sendButton.interactable = CanSend();
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

		bool IsInputValid()
		{
			return _msgInput.text != string.Empty;
		}

		bool CanSend()
		{
			return !_isSending && IsInputValid();
		}

		void OnSendClicked()
		{
			if (!CanSend())
				return;

			var msg = new SentMessageDto
			{
				username = _nick,
				text = _msgInput.text,
			};
			_sendButton.interactable = false;
			_msgInput.interactable = false;

			_ = SendAsync(msg);
		}

		async Task SendAsync(SentMessageDto msg)
		{
			await _client.SendAsync(msg);

			_msgInput.DeactivateInputField();
			_msgInput.text = "";
			_sendButton.interactable = true;
			_msgInput.interactable = true;

			if (!Application.isMobilePlatform)
				_msgInput.ActivateInputField();
		}

		void IMessageHandler.HandleMessage(ReceivedMessageDto msg)
		{
			_msgController.InsertMessage(msg);
		}
	}
}
