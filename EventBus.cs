using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class EventBus
{
    private static Dictionary<Type, SubscribersList<IGlobalSubscriber>> s_Subscribers = new Dictionary<Type, SubscribersList<IGlobalSubscriber>>();

    private static Dictionary<Type, List<Type>> s_CachedSubscriberTypes = new Dictionary<Type, List<Type>>();

    public static void Subscribe(IGlobalSubscriber subscriber)
    {
        List<Type> subscriberTypes = GetSubscriberTypes(subscriber);
        foreach (Type t in subscriberTypes)
        {
            if (!s_Subscribers.ContainsKey(t))
                s_Subscribers[t] = new SubscribersList<IGlobalSubscriber>();
            s_Subscribers[t].Add(subscriber);
        }
    }

    public static void Unsubscribe(IGlobalSubscriber subscriber)
    {
        List<Type> subscriberTypes = GetSubscriberTypes(subscriber);
        foreach (Type t in subscriberTypes)
        {
            if (s_Subscribers.ContainsKey(t))
                s_Subscribers[t].Remove(subscriber);
        }
    }

    public static List<Type> GetSubscriberTypes(IGlobalSubscriber globalSubscriber)
    {
        Type type = globalSubscriber.GetType();

        if (s_CachedSubscriberTypes.ContainsKey(type))
            return s_CachedSubscriberTypes[type];

        List<Type> subsriberTypes = type.GetInterfaces().Where(t => t.GetInterfaces().Contains(typeof(IGlobalSubscriber))).ToList();

        s_CachedSubscriberTypes[type] = subsriberTypes;
        return subsriberTypes;
        
    }

    public static void RaiseEvent<TSubscriber>(Action<TSubscriber> action)
        where TSubscriber : class, IGlobalSubscriber
    {
        SubscribersList<IGlobalSubscriber> subscribers = s_Subscribers[typeof(TSubscriber)];
        subscribers.Executing = true;
        foreach (IGlobalSubscriber subscriber in subscribers.List)
        {
            try
            {
                action.Invoke(subscriber as TSubscriber);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        subscribers.Executing = false;
        subscribers.Cleanup();
    }
}
