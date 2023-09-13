
using Rockstart.Unity.Tut.Chat.Data;
using System.Collections;
using System.Collections.Generic;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public interface IMessageHandler
	{ 
		void HandleMessages(IList<MessageModel> messages);
	}
}
