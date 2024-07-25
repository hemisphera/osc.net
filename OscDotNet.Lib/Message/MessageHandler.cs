using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OscDotNet.Lib;

public sealed class MessageHandler
{
  public Func<MessageHandlerContext, Task> Handler { get; }

  public Regex Regex { get; }


  public MessageHandler(Regex regex, Func<MessageHandlerContext, Task> handler)
  {
    Regex = regex;
    Handler = handler;
  }
}