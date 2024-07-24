using System;

namespace OscDotNet.Lib
{
  public class MalformedMessageException : InvalidOperationException
  {
    public MalformedMessageException(string message, byte[] data)
      : base(message)
    {
      if (data != null)
      {
        MessageData = new byte[data.Length];
        Array.Copy(data, MessageData, data.Length);
      }
    }

    public MalformedMessageException(string message, byte[] data, Exception innerException)
      : base(message, innerException)
    {
      if (data != null)
      {
        MessageData = new byte[data.Length];
        Array.Copy(data, MessageData, data.Length);
      }
    }

    public byte[] MessageData { get; }
  }
}