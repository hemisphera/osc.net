using System;

namespace OscDotNet.Lib;

public class MessageReceivedEventArgs : EventArgs
{
  public MessageReceivedEventArgs(Message message)
  {
    Message = message;
  }

  public Message Message { get; private set; }
}