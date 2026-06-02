namespace KittyTerror.Events
{
    public struct GameOverEvent : IEvent
    {
        public readonly string Reason;

        public GameOverEvent(string reason)
        {
            Reason = reason;
        }
    }
}
