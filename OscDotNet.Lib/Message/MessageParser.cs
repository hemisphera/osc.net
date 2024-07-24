using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OscDotNet.Lib
{
  public class MessageParser
  {
    public Message Parse(byte[] data)
    {
      var builder = new MessageBuilder();
      try
      {
        var byteIndex = 0;
        byteIndex = ParseAddress(data, builder, byteIndex);
        byteIndex = ParseTypeTags(data, builder, byteIndex);
        ParseMessageData(data, builder, byteIndex);

        return builder.ToMessage();
      }
      catch (Exception ex)
      {
        throw new MalformedMessageException("Unable to parse message.", data, ex, builder.Address);
      }
    }

    public byte[] Parse(Message message)
    {
      var builder = new List<byte>();

      SerializeAddress(message, builder);
      SerializeTypeTags(message, builder);
      SerializeMessageData(message, builder);

      return builder.ToArray();
    }

    private int ParseAddress(byte[] data, MessageBuilder builder, int startIndex)
    {
      var addressBuilder = new StringBuilder();
      var byteIndex = startIndex;

      while (byteIndex < data.Length)
      {
        var hasNull = false;

        for (var i = 0; i < 4; i++)
        {
          var val = data[byteIndex];

          if (val > byte.MinValue)
          {
            if (hasNull) throw new MalformedMessageException("Invalid address: address data appearing after null padding at byte position " + byteIndex + ".", data);

            if (val == (byte)',') throw new MalformedMessageException("Invalid address: no null padding before typetags begin at byte position " + byteIndex + ".", data);

            addressBuilder.Append((char)val);
          }
          else
          {
            hasNull = true;
          }

          byteIndex++;
        }

        if (hasNull) break;
      }

      builder.Address = addressBuilder.ToString(); // throws if address is invalid
      return byteIndex;
    }

    private int ParseTypeTags(byte[] data, MessageBuilder builder, int startIndex)
    {
      var byteIndex = startIndex;

      while (byteIndex < data.Length && data[byteIndex] != ',')
        byteIndex++;

      while (byteIndex < data.Length)
      {
        var hasNull = false;

        for (var i = 0; i < 4; i++)
        {
          var val = data[byteIndex];

          if (data[byteIndex] != ',')
          {
            if (val > byte.MinValue)
            {
              if (hasNull) throw new MalformedMessageException("Invalid type tags: type tag data appearing after null padding at byte pos + " + byteIndex + ".", data);

              var typeTag = (TypeTag)val;

              switch (typeTag)
              {
                case TypeTag.OscInt32:
                case TypeTag.OscFloat32:
                case TypeTag.OscString:
                case TypeTag.OscBlob:
                case TypeTag.OscTrue:
                case TypeTag.OscFalse:
                case TypeTag.OscNil:
                  builder.PushAtom(new Atom(typeTag));
                  break;

                default:
                  throw new MalformedMessageException("Unknown/invalid type tag " + typeTag + " at byte pos " + byteIndex + ".", data);
              }
            }
            else
            {
              hasNull = true;
            }
          }

          byteIndex++;
        }

        if (hasNull) break;
      }

      return byteIndex;
    }

    private void ParseMessageData(byte[] data, MessageBuilder builder, int startIndex)
    {
      var byteIndex = startIndex;
      for (var currentAtom = 0; currentAtom < builder.AtomCount; currentAtom++)
      {
        var canIncrement = true;
        var incrementBy = 4;

        switch (builder.GetAtom(currentAtom).TypeTag)
        {
          case TypeTag.OscInt32:
            var intVal = ParseInt32(data, byteIndex);
            builder.SetAtom(currentAtom, intVal);
            break;

          case TypeTag.OscFloat32:
            var floatVal = ParseFloat32(data, byteIndex);
            builder.SetAtom(currentAtom, floatVal);
            break;

          case TypeTag.OscString:
            var stringVal = ParseString(data, byteIndex, ref incrementBy);
            builder.SetAtom(currentAtom, stringVal);
            break;

          case TypeTag.OscBlob:
            var blobVal = ParseBlob(data, byteIndex, ref incrementBy);
            builder.SetAtom(currentAtom, blobVal);
            break;

          case TypeTag.OscNil:
            canIncrement = false;
            break;
        }

        if (canIncrement)
          byteIndex += incrementBy;
      }
    }

    private int ParseInt32(byte[] data, int startPos)
    {
      const int incrementBy = 4;
      var tempPos = startPos;

      if (startPos + incrementBy > data.Length) throw new MalformedMessageException("Missing binary data for int32 at byte index " + startPos + ".", data);

      if (BitConverter.IsLittleEndian)
      {
        var littleEndianBytes = new byte[4];
        littleEndianBytes[3] = data[tempPos++];
        littleEndianBytes[2] = data[tempPos++];
        littleEndianBytes[1] = data[tempPos++];
        littleEndianBytes[0] = data[tempPos++];

        startPos = 0;
        data = littleEndianBytes;
      }

      return BitConverter.ToInt32(data, startPos);
    }

    private float ParseFloat32(byte[] data, int startPos)
    {
      const int incrementBy = 4;
      var tempPos = startPos;

      if (startPos + incrementBy > data.Length) throw new MalformedMessageException("Missing binary data for float32 at byte index " + startPos + ".", data);

      if (BitConverter.IsLittleEndian)
      {
        var littleEndianBytes = new byte[4];
        littleEndianBytes[3] = data[tempPos++];
        littleEndianBytes[2] = data[tempPos++];
        littleEndianBytes[1] = data[tempPos++];
        littleEndianBytes[0] = data[tempPos++];

        startPos = 0;
        data = littleEndianBytes;
      }

      return BitConverter.ToSingle(data, startPos);
    }

    private string ParseString(byte[] data, int startPos, ref int incrementBy)
    {
      if (startPos + 4 > data.Length)
        throw new MalformedMessageException("Missing binary data for string atom at byte index " + startPos + ".", data);

      using var ms = new MemoryStream();
      do
      {
        ms.Write(data, startPos, 4);
        startPos += 4;
      } while (data[startPos - 1] != 0);

      return Encoding.ASCII.GetString(ms.ToArray()).TrimEnd('\0');
    }

    private byte[] ParseBlob(byte[] data, int startPos, ref int incrementBy)
    {
      var length = ParseInt32(data, startPos);

      if (length < 0) throw new MalformedMessageException("Invalid blob length at byte index " + startPos + ".", data);

      startPos += 4;
      incrementBy = 4;

      if (data[startPos] == byte.MinValue)
      {
        startPos += 4;
        incrementBy += 4;
      }

      incrementBy += length;
      incrementBy += 4 - incrementBy % 4;

      if (startPos > data.Length) throw new MalformedMessageException("Missing binary data for blob atom at byte index " + startPos + ".", data);

      var blob = new byte[length];
      Array.Copy(data, startPos, blob, 0, length);
      return blob;
    }

    private void SerializeAddress(Message message, List<byte> builder)
    {
      var count = message.Address.Length;
      builder.AddRange(Encoding.ASCII.GetBytes(message.Address));

      builder.Add(byte.MinValue);
      count++;

      while (count++ % 4 != 0) builder.Add(byte.MinValue);
    }

    private void SerializeTypeTags(Message message, List<byte> builder)
    {
      var count = 0;

      builder.Add((byte)',');
      count++;

      foreach (var typetag in message.TypeTags)
      {
        var b = (byte)typetag;
        builder.Add((byte)typetag);
        count++;
      }

      builder.Add(byte.MinValue);
      count++;

      while (count++ % 4 != 0) builder.Add(byte.MinValue);
    }

    private void SerializeMessageData(Message message, List<byte> builder)
    {
      foreach (var atom in message)
      {
        var type = atom.TypeTag;

        switch (type)
        {
          case TypeTag.OscInt32:
            SerializeInt32(atom.Int32Value, builder);
            break;

          case TypeTag.OscFloat32:
            SerializeFloat32(atom.Float32Value, builder);
            break;

          case TypeTag.OscString:
            SerializeString(atom.StringValue, builder);
            break;

          case TypeTag.OscBlob:
            SerializeBlob(atom.BlobValue, builder);
            break;
        }
      }
    }

    private void SerializeInt32(int value, List<byte> builder)
    {
      var bytes = BitConverter.GetBytes(value);

      if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

      builder.AddRange(bytes);
    }

    private void SerializeFloat32(float value, List<byte> builder)
    {
      var bytes = BitConverter.GetBytes(value);

      if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

      builder.AddRange(bytes);
    }

    private void SerializeString(string value, List<byte> builder)
    {
      SerializeBlob(Encoding.ASCII.GetBytes(value), builder);
    }

    private void SerializeBlob(byte[] value, List<byte> builder)
    {
      SerializeInt32(value.Length, builder);

      builder.AddRange(value);
      builder.Add(byte.MinValue);

      var temp = value.Length + 1;
      while (temp++ % 4 != 0) builder.Add(byte.MinValue);
    }
  }
}