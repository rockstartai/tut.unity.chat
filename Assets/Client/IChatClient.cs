using Cysharp.Threading.Tasks;
using Rockstart.Unity.Tut.Chat.Data;
using System;

namespace Rockstart.Unity.Tut.Chat.Client
{
	public interface IChatClient : IDisposable
	{
		UniTask InitAsync(IMessageHandler messageHandler);
		UniTask SendAsync(MessageModel msg);
	}
}
