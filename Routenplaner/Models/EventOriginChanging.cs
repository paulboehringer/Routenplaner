namespace Routenplaner.Models
{
    public class EventOriginChanging
    {
        public delegate void EventHandler(object sender, OriginChangingEventArgs args);
        public event EventHandler OriginChangingEvent = delegate { };

        public void Publish(OriginChangingEventArgs args)
        {
            OriginChangingEvent(this, args);
        }
    }
}
