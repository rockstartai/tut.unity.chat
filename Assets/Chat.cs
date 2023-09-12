using Cysharp.Threading.Tasks;
using Rockstart.Unity.Tut.Chat.Client;
using Rockstart.Unity.Tut.Chat.Data;
using Rockstart.Unity.Tut.Chat.ScrollView;
using System;
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
			_enterButton.onClick.AddListener(OnStartClicked);
			_sendButton.onClick.AddListener(OnSendClicked);

			_nickInput.onSubmit.AddListener(_ => OnStartClicked());
			_msgInput.onSubmit.AddListener(_ => OnSendClicked());

			_nickInput.ActivateInputField();
		}

		void Update()
		{
			_sendButton.interactable = CanSend();
		}

		void OnStartClicked()
		{
			if (_nickInput.text == string.Empty)
				return;

			InitAsync().Forget();
		}

		async UniTask InitAsync()
		{
			try
			{
				_nickInputPanel.interactable = false;
				await _client.InitAsync(this);
				_nickInputPanel.gameObject.SetActive(false);
				_nick = _nickInput.text;
				_msgController.Init(_nick);
				_msgInput.ActivateInputField();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
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

			var msg = new MessageModel
			{
				username = _nick,
				text = _msgInput.text,
			};

			SendAsync(msg).Forget();
		}

		async UniTask SendAsync(MessageModel msg)
		{
			_msgInput.DeactivateInputField();
			_sendButton.interactable = false;
			_msgInput.interactable = false;

			try
			{
				await _client.SendAsync(msg);
				_msgInput.text = "";
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			_sendButton.interactable = true;
			_msgInput.interactable = true;

			// Allow typing next message immediately if the user has a bigger screen
			if (!Application.isMobilePlatform)
				_msgInput.ActivateInputField();
		}

		void IMessageHandler.HandleMessage(MessageModel msg)
		{
			_msgController.InsertMessage(msg);
		}
	}
}
