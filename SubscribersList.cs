using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribersList<TSubscriber> where TSubscriber : class
{
    private bool m_NeedsCleanUp = false;
    public bool Executing;
    public readonly List<TSubscriber> List = new List<TSubscriber>();

    public void Add(TSubscriber subscriber)
    {
        List.Add(subscriber);
    }

    public void Remove(TSubscriber subscriber)
    {
        if (Executing)
        {
            int i = List.IndexOf(subscriber);
            if (i >= 0)
            {
                m_NeedsCleanUp = true;
                List[i] = null;
            }
        }
        else
        {
            List.Remove(subscriber);
        }
    }

    public void Cleanup()
    {
        if (!m_NeedsCleanUp)
        {
            return;
        }

        List.RemoveAll(s => s == null);
        m_NeedsCleanUp = false;
    }
}
