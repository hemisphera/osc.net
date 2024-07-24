using System.Collections.Generic;

namespace OscDotNet.Lib
{
  public class MessageCreator
  {
    private MessageCreator(string address)
    {
      /*
      if (address == null) throw new ArgumentNullException(nameof(address));
      if (address.Length == 0) throw new ArgumentException("address cannot be empty.", nameof(address));
      if (address[0] != '/') throw new ArgumentException("address must begin with a forward-slash ('/').", nameof(address));
      */
      Address = address;
    }


    public string Address { get; set; }

    public List<Atom> Atoms { get; } = new();

    public static MessageCreator Create()
    {
      return Create("");
    }

    public static MessageCreator Create(string address)
    {
      return new MessageCreator(address);
    }


    public MessageCreator PushAtom(Atom atom)
    {
      Atoms.Add(atom);
      return this;
    }

    public MessageCreator PushAtom(int value)
    {
      return PushAtom(new Atom(value));
    }

    public MessageCreator PushAtom(long value)
    {
      return PushAtom(new Atom(value));
    }

    public MessageCreator PushAtom(float value)
    {
      return PushAtom(new Atom(value));
    }

    public MessageCreator PushAtom(double value)
    {
      return PushAtom(new Atom(value));
    }

    public MessageCreator PushAtom(string value)
    {
      return PushAtom(new Atom(value));
    }

    public MessageCreator PushAtom(byte[] value)
    {
      return PushAtom(new Atom(value));
    }

    public Message ToMessage()
    {
      var typetags = new TypeTag[Atoms.Count];

      for (var i = 0; i < Atoms.Count; i++)
        typetags[i] = Atoms[i].TypeTag;

      return new Message
      {
        Address = Address,
        TypeTags = typetags,
        Atoms = Atoms.ToArray()
      };
    }
  }
}