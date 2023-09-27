using Cysharp.Threading.Tasks;
using com.forbiddenbyte.tut.unity.chat.Data;
using System;

namespace com.forbiddenbyte.tut.unity.chat.Client
{
	public interface IChatClient : IDisposable
	{
		UniTask InitAsync(IMessageHandler messageHandler);
		UniTask SendAsync(MessageModel msg);
	}
}
