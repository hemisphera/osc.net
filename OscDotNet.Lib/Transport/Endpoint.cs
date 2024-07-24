using System.Net;

namespace OscDotNet.Lib
{
  public class OscEndpoint
  {
    public OscEndpoint()
      : this(10000)
    {
    }

    public OscEndpoint(int port)
      : this("127.0.0.1", port)
    {
    }

    public OscEndpoint(string address, int port)
    {
      Address = address;
      Port = port;
    }

    public OscEndpoint(IPEndPoint endpoint)
    {
      Address = endpoint.Address.ToString();
      Port = endpoint.Port;
    }

    public string Address { get; set; }
    public int Port { get; set; }

    public IPEndPoint CreateIpEndpoint()
    {
      var addr = IPAddress.Parse(Address);
      return new IPEndPoint(addr, Port);
    }
  }
}