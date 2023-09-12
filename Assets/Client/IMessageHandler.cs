
using Rockstart.Unity.Tut.Chat.Data;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public interface IMessageHandler
	{ 
		void HandleMessage(MessageModel msg);
	}
}
