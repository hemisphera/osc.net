using System;
using OscDotNet.Lib;
using Xunit;

namespace OscDotNet.Tests.Message;

public class MessageBuilderTests
{
  [Fact]
  public void MessageBuilder_Test_SetAddress_Succeeds()
  {
    var builder = new MessageBuilder();
    Assert.Equal("/", builder.Address);

    builder.Address = "/this/is/a/test";
    Assert.Equal("/this/is/a/test", builder.Address);
  }

  [Fact]
  public void MessageBuilder_Test_SetAddress_Fails_AddressIsNull()
  {
    var builder = new MessageBuilder();
    Assert.Throws<ArgumentNullException>(() => builder.Address = null);
  }

  [Fact]
  public void MessageBuilder_Test_SetAddress_Fails_AddressIsEmpty()
  {
    var builder = new MessageBuilder();
    Assert.Throws<ArgumentException>(() => builder.Address = "");
  }

  [Fact]
  public void MessageBuilder_Test_SetAddress_Fails_AddressIsInvalid()
  {
    var builder = new MessageBuilder();
    Assert.Throws<ArgumentException>(() => builder.Address = "this/is/a/test");
  }

  [Fact]
  public void MessageBuilder_Test_PushPopSetAtoms()
  {
    var builder = new MessageBuilder();

    builder.PushAtom(new Atom(TypeTag.OscInt32));
    builder.PushAtom(new byte[] { 5 });
    builder.PushAtom(5.0f);
    builder.PushAtom(6.0);
    builder.PushAtom(7);
    builder.PushAtom(8L);
    builder.PushAtom("Test");

    Assert.Equal(7, builder.AtomCount);

    var last = builder.PopAtom();
    Assert.Equal(6, builder.AtomCount);
    Assert.Equal("Test", last);

    builder.SetAtom(5, 1234);
    Assert.Equal(1234, builder.GetAtom(5));
  }

  [Fact]
  public void MessageBuilder_Test_PopAtom_Fails_CollectionEmpty()
  {
    var builder = new MessageBuilder();
    Assert.Throws<InvalidOperationException>(() => builder.PopAtom());
  }

  [Fact]
  public void MessageBuilder_Test_SetAtom_Fails_InvalidIndex()
  {
    var builder = new MessageBuilder();
    Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetAtom(0, "test"));
  }

  [Fact]
  public void MessageBuilder_Test_Reset()
  {
    var builder = new MessageBuilder
    {
      Address = "/test"
    };
    builder.PushAtom(1234);

    Assert.Equal("/test", builder.Address);
    Assert.Equal(1, builder.AtomCount);

    builder.Reset();
    Assert.Equal("/", builder.Address);
    Assert.Equal(0, builder.AtomCount);
  }

  [Fact]
  public void MessageBuilder_Test_ToMessage()
  {
    var builder = new MessageBuilder
    {
      Address = "/test"
    };
    builder.PushAtom(new Atom(TypeTag.OscInt32));
    builder.PushAtom(new byte[] { 5 });
    builder.PushAtom(5.0f);
    builder.PushAtom(6.0);
    builder.PushAtom(7);
    builder.PushAtom(8L);
    builder.PushAtom("Test");

    var message = builder.ToMessage();

    Assert.Equal("/test", message.Address);
    Assert.Equal(7, message.Atoms.Length);
    Assert.Equal(TypeTag.OscInt32, message.Atoms[0].TypeTag);
    Assert.Equal(TypeTag.OscBlob, message.Atoms[1].TypeTag);
    Assert.Equal(TypeTag.OscFloat32, message.Atoms[2].TypeTag);
    Assert.Equal(TypeTag.OscFloat32, message.Atoms[3].TypeTag);
    Assert.Equal(TypeTag.OscInt32, message.Atoms[4].TypeTag);
    Assert.Equal(TypeTag.OscInt32, message.Atoms[5].TypeTag);
    Assert.Equal(TypeTag.OscString, message.Atoms[6].TypeTag);
  }
}