using Rockstart.Unity.Tut.Chat.ScrollView;
using System.Threading.Tasks;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public interface IChatClient
	{ 
		Task InitAsync();
		void SetMessageHandler(IMessageHandler messageHandler);
		Task SendAsync(SentMessageDto msg);
	}
}
