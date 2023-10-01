
using Com.ForbiddenByte.Tut.Unity.Chat.Data;
using System.Collections;
using System.Collections.Generic;

namespace Com.ForbiddenByte.Tut.Unity.Chat.Client
{
	public interface IMessageHandler
	{ 
		void HandleMessages(IList<MessageModel> messages);
	}
}
