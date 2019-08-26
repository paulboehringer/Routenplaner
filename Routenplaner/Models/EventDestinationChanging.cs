namespace Routenplaner.Models
{
    /// <summary>
    /// Used to notify the RoutenViewmodel of changes on field Destination
    /// </summary>
    public class EventDestinationChanging
    {
        public delegate void EventHandler(object sender, FieldChangingEventArgs args);
        public event EventHandler DestinationChangingEvent = delegate { };

        public void Publish(FieldChangingEventArgs args)
        {
            DestinationChangingEvent(this, args);
        }
    }
}
