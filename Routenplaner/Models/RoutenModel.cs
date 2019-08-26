using Prism.Mvvm;
using Services.Core;
using System.Collections.Generic;
using System.Windows.Media;

namespace Routenplaner.Models
{
    /// <summary>
    /// The model for the view RoutenView
    /// </summary>
    public class RoutenModel : BindableBase
    {
        private EventOriginChanging origChangingEvent;
        private EventDestinationChanging destChangingEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="origChangingEvent"></param>
        /// <param name="destChangingEvent"></param>
        public RoutenModel(EventOriginChanging origChangingEvent, EventDestinationChanging destChangingEvent)
        {
            this.origChangingEvent = origChangingEvent;
            this.destChangingEvent = destChangingEvent;
            Waypoints = new List<LocationDto>();
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
                origChangingEvent.Publish(new FieldChangingEventArgs() { CanDisplay = !string.IsNullOrEmpty(origin) });
                destChangingEvent.Publish(new FieldChangingEventArgs()
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

        private List<LocationDto> waypoints;

        public List<LocationDto> Waypoints
        {
            get
            {
                return waypoints;
            }
            set
            {
                SetProperty(ref waypoints, value);
                RaisePropertyChanged(nameof(Waypoints));
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
                destChangingEvent.Publish(new FieldChangingEventArgs()
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
