using System;

namespace KittyTerror.Events
{
    public static class EventBus<T> where T : IEvent
    {
        public static event Action<T> OnRaised;

        public static void Raise(T eventData)
        {
            OnRaised?.Invoke(eventData);
        }
    }
}
