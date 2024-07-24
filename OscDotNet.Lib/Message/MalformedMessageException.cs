using System;

namespace OscDotNet.Lib
{
  public class MalformedMessageException : InvalidOperationException
  {
    public MalformedMessageException(string message, byte[] data, string address = null)
      : base(message)
    {
      MessageData = data;
      Address = address;
    }

    public MalformedMessageException(string message, byte[] data, Exception innerException, string address = null)
      : base(message, innerException)
    {
      MessageData = data;
      Address = address;
    }

    public byte[] MessageData { get; }

    public string Address { get; }
  }
}