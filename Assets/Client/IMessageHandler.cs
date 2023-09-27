
using com.forbiddenbyte.tut.unity.chat.Data;
using System.Collections;
using System.Collections.Generic;

namespace com.forbiddenbyte.tut.unity.chat.Client
{
	public interface IMessageHandler
	{ 
		void HandleMessages(IList<MessageModel> messages);
	}
}
