namespace Routenplaner.Models
{
    public class EventDestinationChanging
    {
        public delegate void EventHandler(object sender, OriginChangingEventArgs args);
        public event EventHandler DestinationChangingEvent = delegate { };

        public void Publish(OriginChangingEventArgs args)
        {
            DestinationChangingEvent(this, args);
        }
    }
}
