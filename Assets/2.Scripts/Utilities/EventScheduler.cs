using System;
using System.Collections.Generic;
using UnityEngine;

public class EventScheduler : MonoBehaviourSingleton<EventScheduler>
{
    private Dictionary<string, Action> repeatingEvents = new Dictionary<string, Action>();
    private List<ScheduledEvent> scheduledEvents = new List<ScheduledEvent>();

    void Update()
    {
        float currentTime = Time.time;
        scheduledEvents.RemoveAll(eventData =>
        {
            bool shouldRemove = currentTime >= eventData.triggerTime;
            if (shouldRemove)
            {
                eventData.callback?.Invoke();
            }
            return shouldRemove;
        });
    }

    /// <summary>
    /// ScheduleEvent method schedules a one-time event to be triggered after the given delay
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public string ScheduleRepeatingEvent(float interval, Action callback)
    {
        string eventId = Guid.NewGuid().ToString();
        repeatingEvents.Add(eventId, callback);
        InvokeRepeating(eventId, interval);
        return eventId;
    }

    /// <summary>
    /// StopRepeatingEvent method stops the repeating event with the given eventId
    /// </summary>
    /// <param name="eventId"></param>
    public void StopRepeatingEvent(string eventId)
    {
        if (repeatingEvents.ContainsKey(eventId))
        {
            // Additionally, remove any scheduled events associated with this ID
            scheduledEvents.RemoveAll(e => e.callback == repeatingEvents[eventId]);
            repeatingEvents.Remove(eventId);
        }
    }

    private void InvokeRepeating(string eventId, float interval)
    {
        if (repeatingEvents.TryGetValue(eventId, out Action callback))
        {
            callback.Invoke();
            // Schedule the next invocation
            ScheduledEvent scheduledEvent = new ScheduledEvent(Time.time + interval, () => InvokeRepeating(eventId, interval));
            scheduledEvents.Add(scheduledEvent);
        }
    }
}
