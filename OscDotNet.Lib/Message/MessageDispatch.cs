using System;
using System.Collections.Generic;

namespace OscDotNet.Lib
{
  public class MessageDispatch
  {
    private readonly Dictionary<string, List<Action<Message>>> dispatchTable = new();

    public void RegisterMethod(string oscMethod, Action<Message> callback)
    {
      if (dispatchTable.ContainsKey(oscMethod))
        dispatchTable[oscMethod].Add(callback);
      else
        dispatchTable[oscMethod] = new List<Action<Message>>
        {
          callback
        };
    }

    public void UnregisterMethod(string oscMethod)
    {
      dispatchTable.Remove(oscMethod);
    }

    public void UnregisterCallback(string oscMethod, Action<Message> callback)
    {
      if (dispatchTable.ContainsKey(oscMethod)) dispatchTable[oscMethod].Remove(callback);
    }

    public void UnregisterCallback(Action<Message> callback)
    {
      foreach (var callbackList in dispatchTable.Values) callbackList.Remove(callback);
    }

    public void Dispatch(Message message)
    {
      if (dispatchTable.ContainsKey(message.Address))
      {
        var callbackList = dispatchTable[message.Address];
        foreach (var callback in callbackList) callback(message);
      }

      // TODO : partial address matching dispatch
    }
  }
}