using System;
using System.Net.Sockets;

namespace OscDotNet.Lib
{
  public delegate void OnMessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

  public interface IOscServer : IDisposable
  {
    OscEndpoint Endpoint { get; }
    event OnMessageReceivedEventHandler MessageReceived;

    void BeginListen();
    void EndListen();
  }

  public class MessageReceivedEventArgs : EventArgs
  {
    public MessageReceivedEventArgs(Message message)
    {
      Message = message;
    }

    public Message Message { get; private set; }
  }

  public class OscUdpServer : IOscServer
  {
    private static readonly MessageParser defaultMessageParser = new();
    private bool islistening;
    private readonly Socket socket;

    public OscUdpServer(OscEndpoint endpoint)
    {
      Endpoint = endpoint;
      socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

      socket.Bind(
        Endpoint.CreateIpEndpoint()
      );
    }

    public OscEndpoint Endpoint { get; }
    public event OnMessageReceivedEventHandler MessageReceived;

    public void BeginListen()
    {
      if (!islistening)
      {
        islistening = true;
        OnListen();
      }
    }

    public void EndListen()
    {
      if (islistening) islistening = false;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
    {
      var temp = MessageReceived;

      if (temp != null) temp(this, args);
    }


    private void OnListen()
    {
      if (!islistening) return;

      var buffer = new byte[8096];
      socket.BeginReceive(
        buffer,
        0,
        buffer.Length,
        SocketFlags.None,
        ia =>
        {
          var bytesReceived = socket.EndReceive(ia);

          try
          {
            if (bytesReceived > 0)
            {
              var byteBuffer = (byte[])ia.AsyncState;
              var msg = defaultMessageParser.Parse(byteBuffer);

              OnMessageReceived(
                new MessageReceivedEventArgs(msg)
              );
            }

            OnListen();
          }
          catch (MalformedMessageException)
          {
            OnListen();
            throw;
          }
          catch
          {
            islistening = false;
            throw;
          }
        },
        buffer);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing) EndListen();
    }
  }
}