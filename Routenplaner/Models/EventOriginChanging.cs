namespace Routenplaner.Models
{
    /// <summary>
    /// Used to notify the RoutenViewmodel of changes on field Origin
    /// </summary>
    public class EventOriginChanging
    {
        public delegate void EventHandler(object sender, FieldChangingEventArgs args);
        public event EventHandler OriginChangingEvent = delegate { };

        public void Publish(FieldChangingEventArgs args)
        {
            OriginChangingEvent(this, args);
        }
    }
}
