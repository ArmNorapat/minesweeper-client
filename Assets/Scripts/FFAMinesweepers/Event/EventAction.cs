using System;

namespace TrueAxion.FFAMinesweepers.Event
{
    public class EventAction<T>
    {
        private event Action<T> eventAction;

        public void SubscribeToEvent(Action<T> action)
        {
            eventAction += action;
        }

        public void UnsubscribeToEvent(Action<T> action)
        {
            eventAction -= action;
        }

        internal void FireEvent(T value)
        {
            eventAction?.Invoke(value);
        }
    }

    public class EventAction
    {
        private event Action eventAction;

        public void SubscribeToEvent(Action action)
        {
            eventAction += action;
        }
        
        public void UnsubscribeToEvent(Action action)
        {
            eventAction -= action;
        }

        internal void FireEvent()
        {
            eventAction?.Invoke();
        }
    }
}