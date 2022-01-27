using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    public readonly EventType eventType;

    public EventData(EventType type)
    {
        eventType = type;
    }
}
