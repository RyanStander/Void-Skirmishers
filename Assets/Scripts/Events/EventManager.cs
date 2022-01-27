using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    //------------------------------------------------------------------------------------------------------------------------
    //                                                  Singleton
    //------------------------------------------------------------------------------------------------------------------------
    //Getter and setter for the current manager, static so that there is only one manager at any given time
    private static EventManager currentManager { get; set; }

    //Awake function ensures that only one copy exists in the scene at a given time
    private void Awake()
    {
        if (currentManager == null)
        {
            currentManager = this; //Sets the active manager to this instance of it
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //------------------------------------------------------------------------------------------------------------------------
    //                                                 Event Queue Methods
    //------------------------------------------------------------------------------------------------------------------------
    public delegate void EventHandler(EventData eventData); //Define the method signature
    private static Dictionary<EventType, EventHandler> subscriberDictionary = new Dictionary<EventType, EventHandler>(); //Holds a dictionary of all the subscribers
    private static Queue<EventData> eventQueue = new Queue<EventData>(); //Queues up the events that are coming in

    public static void Subscribe(EventType eventType, EventHandler eventHandler)
    {
        //If key is not already present, then add a new key and value pair (event, handler)
        if (!subscriberDictionary.ContainsKey(eventType))
        {
            EventHandler handler = null;
            subscriberDictionary.Add(eventType, handler);
        }
        //Adds the specific handler to the assosiated key
        subscriberDictionary[eventType] += eventHandler;
    }

    public static void Unsubscribe(EventType eventType, EventHandler eventHandler)
    {
        //If key is already present, then remove the key and value pair (event, handler)
        if (subscriberDictionary.ContainsKey(eventType))
        {
            //If event handler is not present, following code will get ignored
            subscriberDictionary[eventType] -= eventHandler;
        }
        else
        {
            //Throw an error (Log file)
            Console.WriteLine("Warning: Event type " + eventType.ToString()+" doesn't exist in the event manager's subscriber dictionary");

            //Unity Player
            Debug.Log("Warning: Event type " + eventType.ToString() + " doesn't exist in the event manager's subscriber dictionary");
        }
    }
    public static void AddEvent(EventData eventData)
    {
        //Error handling
        if (!Enum.IsDefined(typeof(EventType),eventData.eventType))
        {
            throw new ArgumentOutOfRangeException(eventData.eventType.ToString(),"Event type is invalid");
        }

        //Add the event to the queue so that it may be proccesed 
        eventQueue.Enqueue(eventData);
    }
    //Iterates through the event queue, invokes the event, and then removes the event.
    public static void PublishEvents()
    {
        //Forloop for all current queued up events
        for (int i = eventQueue.Count - 1; i >=0;i--)
        {
            //Peeks the frontward item of the queue
            EventData data = eventQueue.Peek();

            //Check if the dictionary already contains this event type
            if (subscriberDictionary.ContainsKey(data.eventType))
            {
                //Invoke/Fire off the event for all of its listeners
                subscriberDictionary[data.eventType]?.Invoke(data);
            }
            else
            {
                //Throw an error (Log file)
                Console.WriteLine("Warning: Event type " + data.ToString() + " doesn't exist in the event manager's subscriber dictionary");

                //Unity Player
                Debug.Log("Warning: Event type " + data.ToString() + " doesn't exist in the event manager's subscriber dictionary");
            }
            //Remove the event from the queue now that it has been resolved
            eventQueue.Dequeue();
        }
    }
    public void Update()
    {
        PublishEvents();
    }
}
