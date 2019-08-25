using Prism.Mvvm;
using System.Windows.Media;

namespace Routenplaner.Models
{
    public class RoutenModel : BindableBase
    {
        private EventOriginChanging origChangingEvent;
        private EventDestinationChanging destChangingEvent;

        public RoutenModel(EventOriginChanging origChangingEvent, EventDestinationChanging destChangingEvent)
        {
            this.origChangingEvent = origChangingEvent;
            this.destChangingEvent = destChangingEvent;
        }

        #region Properties

        private string origin;

        public string Origin
        {
            get
            {
                return origin;
            }
            set
            {
                if (origin == value) { return; }
                SetProperty(ref origin, value);
                RaisePropertyChanged(nameof(Origin));
                origChangingEvent.Publish(new OriginChangingEventArgs() { CanDisplay = !string.IsNullOrEmpty(origin) });
                destChangingEvent.Publish(new OriginChangingEventArgs()
                {
                    CanDisplay = !string.IsNullOrEmpty(origin) && !string.IsNullOrEmpty(destination)
                });
            }
        }

        private string waypoint;

        public string Waypoint
        {
            get
            {
                return waypoint;
            }
            set
            {
                SetProperty(ref waypoint, value);
                RaisePropertyChanged(nameof(Waypoint));
            }
        }

        private string destination;

        public string Destination
        {
            get
            {
                return destination;
            }
            set
            {
                RaisePropertyChanged(nameof(Destination));
                if (destination == value) { return; }
                SetProperty(ref destination, value);
                destChangingEvent.Publish(new OriginChangingEventArgs()
                {
                    CanDisplay = !string.IsNullOrEmpty(origin) && !string.IsNullOrEmpty(destination)
                });
            }
        }

        private ImageSource mapView;

        public ImageSource MapView
        {
            get
            {
                return mapView;
            }
            set
            {
                SetProperty(ref mapView, value);
                RaisePropertyChanged(nameof(MapView));
            }
        }

        private string directions;
        public string Directions
        {
            get
            {
                return directions;
            }
            set
            {
                SetProperty(ref directions, value);
                RaisePropertyChanged(nameof(Directions));
            }
        }

        private string error;

        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                SetProperty(ref error, value);
                RaisePropertyChanged(nameof(Error));
            }
        }
        #endregion Properties

    }
}
