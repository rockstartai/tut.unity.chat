using Cysharp.Threading.Tasks;
using com.forbiddenbyte.tut.unity.chat.Client;
using com.forbiddenbyte.tut.unity.chat.Data;
using com.forbiddenbyte.tut.unity.chat.ScrollView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace com.forbiddenbyte.tut.unity.chat
{
	public class Chat : MonoBehaviour, IMessageHandler
	{
		public TMPro.TMP_InputField nickInput;
		public Button enterButton;
		public CanvasGroup nickInputPanel;
		public MessagesController msgController;
		public TMPro.TMP_InputField msgInput;
		public Button sendButton;

		bool _isSending;
		IChatClient _client;
		string _nick;


		void Start()
		{
			_client = GetComponent<IChatClient>();
			enterButton.onClick.AddListener(OnStartClicked);
			sendButton.onClick.AddListener(OnSendClicked);

			nickInput.onSubmit.AddListener(_ => OnStartClicked());
			msgInput.onSubmit.AddListener(_ => OnSendClicked());

			nickInput.ActivateInputField();
		}

		void Update()
		{
			sendButton.interactable = CanSend();
		}

		void OnStartClicked()
		{
			if (nickInput.text == string.Empty)
				return;

			InitAsync().Forget();
		}

		async UniTask InitAsync()
		{
			try
			{
				nickInputPanel.interactable = false;
				await _client.InitAsync(this);
				nickInputPanel.gameObject.SetActive(false);
				_nick = nickInput.text;
				msgController.Init(_nick);
				msgInput.ActivateInputField();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		bool IsInputValid()
		{
			return msgInput.text != string.Empty;
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
				text = msgInput.text,
			};

			SendAsync(msg).Forget();
		}

		async UniTask SendAsync(MessageModel msg)
		{
			msgInput.DeactivateInputField();
			sendButton.interactable = false;
			msgInput.interactable = false;

			try
			{
				await _client.SendAsync(msg);
				msgInput.text = "";
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			sendButton.interactable = true;
			msgInput.interactable = true;

			// Allow typing next message immediately if the user has a bigger screen
			if (!Application.isMobilePlatform)
				msgInput.ActivateInputField();
		}

		void IMessageHandler.HandleMessages(IList<MessageModel> messages)
		{
			msgController.InsertMessages(messages);
		}
	}
}
