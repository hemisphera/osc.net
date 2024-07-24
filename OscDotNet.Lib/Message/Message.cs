using System.Collections;
using System.Collections.Generic;

namespace OscDotNet.Lib
{
  public class Message : IEnumerable<Atom>
  {
    internal Message()
    {
    }

    public string Address { get; set; }
    public TypeTag[] TypeTags { get; set; }
    public Atom[] Atoms { get; set; }

    public override bool Equals(object obj)
    {
      return Equals(obj as Message);
    }

    public virtual bool Equals(Message rhs)
    {
      if (rhs == null) return false;

      if (Address != rhs.Address) return false;
      if (Atoms == null)
        return rhs.Atoms == null;
      if (rhs.Atoms == null) return false;

      if (Atoms.Length != rhs.Atoms.Length) return false;

      for (var i = 0; i < Atoms.Length; i++)
        if (!Atoms[i].Equals(rhs.Atoms[i]))
          return false;

      return true;
    }

    public override int GetHashCode()
    {
      var hashCode = Address == null ? 0 : Address.GetHashCode();
      if (Atoms != null)
        foreach (var atom in Atoms)
          hashCode ^= atom.GetHashCode();

      return hashCode;
    }

    #region IEnumerable<Atom> Members

    public IEnumerator<Atom> GetEnumerator()
    {
      foreach (var atom in Atoms ?? new Atom[0]) yield return atom;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}