using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OscDotNet.Lib;

public interface IOscServer : IDisposable
{
  OscEndpoint Endpoint { get; }

  event EventHandler<MessageReceivedEventArgs> MessageReceived;

  void BeginListen();

  void EndListen();

  void RegisterHandler(Regex re, Func<MessageHandlerContext, Task> handler);
}