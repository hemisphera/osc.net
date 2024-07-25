using System;
using System.Collections.Generic;

namespace OscDotNet.Lib
{
  public class MessageBuilder
  {
    private string _address = "/";
    private readonly List<Atom> _atoms = [];


    public string Address
    {
      get => _address;
      set
      {
        if (value == null) throw new ArgumentNullException(nameof(Address));
        if (value.Length == 0) throw new ArgumentException("address cannot be empty.", nameof(Address));
        if (value[0] != '/')
          throw new ArgumentException("address must begin with a forward-slash ('/').", nameof(Address));
        _address = value;
      }
    }

    public int AtomCount => _atoms.Count;

    public Atom GetAtom(int index)
    {
      return _atoms[index];
    }

    public void PushAtom(Atom atom)
    {
      _atoms.Add(atom);
    }

    public void PushAtom(int value)
    {
      _atoms.Add(new Atom(value));
    }

    public void PushAtom(long value)
    {
      _atoms.Add(new Atom(value));
    }

    public void PushAtom(float value)
    {
      _atoms.Add(new Atom(value));
    }

    public void PushAtom(double value)
    {
      _atoms.Add(new Atom(value));
    }

    public void PushAtom(string value)
    {
      _atoms.Add(new Atom(value));
    }

    public void PushAtom(byte[] value)
    {
      _atoms.Add(new Atom(value));
    }

    public Atom PopAtom()
    {
      if (_atoms.Count == 0) throw new InvalidOperationException("No Atom to pop.");

      var idx = _atoms.Count - 1;
      var atom = _atoms[idx];
      _atoms.RemoveAt(idx);
      return atom;
    }

    public void SetAtom(int index, int value)
    {
      _atoms[index] = new Atom(value);
    }

    public void SetAtom(int index, float value)
    {
      _atoms[index] = new Atom(value);
    }

    public void SetAtom(int index, string value)
    {
      _atoms[index] = new Atom(value);
    }

    public void SetAtom(int index, byte[] value)
    {
      _atoms[index] = new Atom(value);
    }

    public void Reset()
    {
      _address = "/";
      _atoms.Clear();
    }

    public Message ToMessage()
    {
      var typetags = new TypeTag[_atoms.Count];

      for (var i = 0; i < _atoms.Count; i++) typetags[i] = _atoms[i].TypeTag;

      return new Message
      {
        Address = _address,
        TypeTags = typetags,
        Atoms = _atoms.ToArray()
      };
    }
  }
}