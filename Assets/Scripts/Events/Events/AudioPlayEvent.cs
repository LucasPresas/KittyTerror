namespace KittyTerror.Events
{
    public struct AudioPlayEvent : IEvent
    {
        public readonly string ClipId;

        public AudioPlayEvent(string clipId)
        {
            ClipId = clipId;
        }
    }
}
