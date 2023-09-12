using Rockstart.Unity.Tut.Chat.Data;
using System;
using UnityEngine;

namespace Rockstart.Unity.Tut.Chat.ScrollView
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

		public void InsertMessage(MessageModel msg)
		{
			var prefab = PickPrefabFor(msg);
			var instanceGo = Instantiate(prefab, _content, worldPositionStays: false);
			var view = instanceGo.GetComponent<MessageView>();
			view.UpdateViews(msg);
		}

		GameObject PickPrefabFor(MessageModel msg)
		{
			if (msg.username == _nick)
				return _msgPrefabMe.gameObject;

			return _msgPrefabOther.gameObject;
		}
	}
}
