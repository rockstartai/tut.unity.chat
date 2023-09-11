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

		public void InsertMessage(MessageModel model)
		{
			var prefab = PickPrefabFor(model);
			var instanceGo = Instantiate(prefab, _content, worldPositionStays: false);
			var view = instanceGo.GetComponent<MessageView>();
			view.UpdateViews(model);
		}

		GameObject PickPrefabFor(MessageModel model)
		{
			if (model.sender == _nick)
				return _msgPrefabMe.gameObject;

			return _msgPrefabOther.gameObject;
		}
	}
}
