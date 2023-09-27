using com.forbiddenbyte.tut.unity.chat.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.forbiddenbyte.tut.unity.chat.ScrollView
{
	public class MessagesController : MonoBehaviour
	{
		[SerializeField] RectTransform _content;
		[SerializeField] RectTransform _msgPrefabMe;
		[SerializeField] RectTransform _msgPrefabOther;

		string _nick;


		public void Init(string nick)
		{
			_nick = nick;
		}

		public void InsertMessages(IList<MessageModel> messages)
		{
			foreach (var msg in messages)
			{
				var prefab = PickPrefabFor(msg);
				var instanceGo = Instantiate(prefab, _content, worldPositionStays: false);
				var view = instanceGo.GetComponent<MessageView>();
				view.UpdateViews(msg);
			}
		}

		GameObject PickPrefabFor(MessageModel msg)
		{
			if (msg.username == _nick)
				return _msgPrefabMe.gameObject;

			return _msgPrefabOther.gameObject;
		}
	}
}
