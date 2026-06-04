namespace KittyTerror.Events
{
    public struct ThoughtEvent : IEvent
    {
        public readonly string ThoughtId;

        public ThoughtEvent(string thoughtId)
        {
            ThoughtId = thoughtId;
        }
    }
}
