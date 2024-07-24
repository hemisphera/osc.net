using System;
using System.Collections.Generic;

namespace OscDotNet.Lib
{
  public class MessageBuilder
  {
    private string address = "/";
    private readonly List<Atom> atoms = new();


    public string Address
    {
      get => address;
      set
      {
        if (value == null) throw new ArgumentNullException(nameof(Address));
        if (value.Length == 0) throw new ArgumentException("address cannot be empty.", nameof(Address));
        if (value[0] != '/')
          throw new ArgumentException("address must begin with a forward-slash ('/').", nameof(Address));
        address = value;
      }
    }

    public int AtomCount => atoms.Count;

    public Atom GetAtom(int index)
    {
      return atoms[index];
    }

    public void PushAtom(Atom atom)
    {
      atoms.Add(atom);
    }

    public void PushAtom(int value)
    {
      atoms.Add(new Atom(value));
    }

    public void PushAtom(long value)
    {
      atoms.Add(new Atom(value));
    }

    public void PushAtom(float value)
    {
      atoms.Add(new Atom(value));
    }

    public void PushAtom(double value)
    {
      atoms.Add(new Atom(value));
    }

    public void PushAtom(string value)
    {
      atoms.Add(new Atom(value));
    }

    public void PushAtom(byte[] value)
    {
      atoms.Add(new Atom(value));
    }

    public Atom PopAtom()
    {
      if (atoms.Count == 0) throw new InvalidOperationException("No Atom to pop.");

      var idx = atoms.Count - 1;
      var atom = atoms[idx];
      atoms.RemoveAt(idx);
      return atom;
    }

    public void SetAtom(int index, int value)
    {
      atoms[index] = new Atom(value);
    }

    public void SetAtom(int index, float value)
    {
      atoms[index] = new Atom(value);
    }

    public void SetAtom(int index, string value)
    {
      atoms[index] = new Atom(value);
    }

    public void SetAtom(int index, byte[] value)
    {
      atoms[index] = new Atom(value);
    }

    public void Reset()
    {
      address = "/";
      atoms.Clear();
    }

    public Message ToMessage()
    {
      var typetags = new TypeTag[atoms.Count];

      for (var i = 0; i < atoms.Count; i++) typetags[i] = atoms[i].TypeTag;

      return new Message
      {
        Address = address,
        TypeTags = typetags,
        Atoms = atoms.ToArray()
      };
    }
  }
}