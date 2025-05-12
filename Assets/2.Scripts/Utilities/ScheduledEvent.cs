using System;

public class ScheduledEvent
{
    public float triggerTime;
    public Action callback;

    public ScheduledEvent(float triggerTime, Action callback)
    {
        this.triggerTime = triggerTime;
        this.callback = callback;
    }
}
