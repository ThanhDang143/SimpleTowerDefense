using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationService : Singleton<NotificationService>
{
    public delegate void UpdateDelegator();
    public delegate void Delegator(Hashtable data);

    private Dictionary<string, UpdateDelegator> _delegateMap;
    private Dictionary<string, Delegator> _maps;

    public NotificationService()
    {
        _delegateMap = new Dictionary<string, UpdateDelegator>();
        _maps = new Dictionary<string, Delegator>();
    }

    public void Add(string subject, UpdateDelegator delegator)
    {
        _delegateMap.TryAdd(subject, delegate () { });
        _delegateMap[subject] += delegator;
    }

    public void Add(string subject, Delegator delegator)
    {
        _maps.TryAdd(subject, data => { });
        _maps[subject] += delegator;
    }

    public void Remove(string subject, UpdateDelegator delegator)
    {
        if (_delegateMap.ContainsKey(subject) == false) return;
        _delegateMap[subject] -= delegator;
    }

    public void Remove(string subject, Delegator delegator)
    {
        if (_maps.ContainsKey(subject) == false) return;
        _maps[subject] -= delegator;
    }

    public void Post(string subject)
    {
        if (_delegateMap.TryGetValue(subject, out var value))
        {
            if (value != null)
                foreach (var @delegate in value.GetInvocationList())
                {
                    var delegator = (UpdateDelegator)@delegate;
                    try
                    {
                        delegator();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
        }
    }

    public void Post(string subject, Hashtable data)
    {
        if (_maps.TryGetValue(subject, out var map))
        {
            if (map != null)
                foreach (var @delegate in map.GetInvocationList())
                {
                    var delegator = (Delegator)@delegate;
                    try
                    {
                        delegator.Invoke(data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
        }
    }
}
