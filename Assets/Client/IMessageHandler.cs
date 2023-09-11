
namespace Rockstart.Unity.Tut.Chat.Client
{
	public interface IMessageHandler
	{ 
		void HandleMessage(ReceivedMessageDto msg);
	}
}
