using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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

    //private Socket socket;
    private readonly UdpClient _client;
    private CancellationTokenSource _token;
    private bool islistening;

    public OscUdpServer(OscEndpoint endpoint)
    {
      Endpoint = endpoint;
      _client = new UdpClient(endpoint.CreateIpEndpoint());
    }

    public OscEndpoint Endpoint { get; }

    public event OnMessageReceivedEventHandler MessageReceived;

    public void BeginListen()
    {
      if (!islistening)
      {
        islistening = true;
        _token = new CancellationTokenSource();
        Task.Run(async () => await ListenLoop());
      }
    }

    public void EndListen()
    {
      if (islistening)
      {
        _token.Cancel();
        islistening = false;
      }
    }

    /*
        private void OnListen() {
            if (!islistening) return;

            var buffer = new byte[8096];
            socket.BeginReceive(
                buffer,
                0,
                buffer.Length,
                SocketFlags.None,
                (ia) => {
                    int bytesReceived = socket.EndReceive(ia);

                    try {
                        if (bytesReceived > 0) {
                            var byteBuffer = (byte[])ia.AsyncState;
                            Message msg = defaultMessageParser.Parse(byteBuffer);

                            OnMessageReceived(
                                new MessageReceivedEventArgs(msg)
                                );
                        }

                        OnListen();
                    }
                    catch (MalformedMessageException) {
                        OnListen();
                        throw;
                    }
                    catch {
                        islistening = false;
                        throw;
                    }
                },
                buffer);
        }
    */

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    public event EventHandler<Exception> MessageFailed;

    private async Task ListenLoop()
    {
      while (!_token.IsCancellationRequested)
        try
        {
          var data = await _client.ReceiveAsync();
          var msg = defaultMessageParser.Parse(data.Buffer);
          MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg));
        }
        catch (MalformedMessageException ex)
        {
          MessageFailed?.Invoke(this, ex);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing) EndListen();
    }
  }
}