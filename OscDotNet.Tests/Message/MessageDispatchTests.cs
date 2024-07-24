using System;
using OscDotNet.Lib;
using Xunit;

namespace OscDotNet.Tests.Message
{
  public class MessageDispatchTests
  {
    [Fact]
    public void MessageDispatch_Test_Dispatch()
    {
      var dispatch = new MessageDispatch();
      var builder1 = new MessageBuilder
      {
        Address = "/test1"
      };
      builder1.PushAtom(1);

      var builder2 = new MessageBuilder
      {
        Address = "/test2"
      };
      builder2.PushAtom("TEST");

      var builder3 = new MessageBuilder
      {
        Address = "/test3"
      };
      builder3.PushAtom("TEST2");

      var address = "";
      var value = new Atom();

      Action<Lib.Message> callback = m =>
      {
        address = m.Address;
        value = m.Atoms[0];
      };

      dispatch.RegisterMethod("/test1", callback);
      dispatch.RegisterMethod("/test2", callback);

      // Test
      dispatch.Dispatch(builder1.ToMessage());
      Assert.Equal("/test1", address);
      Assert.Equal(1, value);

      dispatch.Dispatch(builder2.ToMessage());
      Assert.Equal("/test2", address);
      Assert.Equal("TEST", value);

      // No callback registered, values should not be set
      dispatch.Dispatch(builder3.ToMessage());
      Assert.NotEqual("/test3", address);
      Assert.NotEqual("TEST2", value);
    }
  }
}