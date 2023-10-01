using Cysharp.Threading.Tasks;
using Com.ForbiddenByte.Tut.Unity.Chat.Data;
using System;

namespace Com.ForbiddenByte.Tut.Unity.Chat.Client
{
	public interface IChatClient : IDisposable
	{
		UniTask InitAsync(IMessageHandler messageHandler);
		UniTask SendAsync(MessageModel msg);
	}
}
