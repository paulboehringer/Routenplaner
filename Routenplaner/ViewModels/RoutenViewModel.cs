using Prism.Commands;
using Prism.Mvvm;
using Routenplaner.Models;
using Services.Core;
using Services.Directions;
using Services.GeoCoding;
using Services.StaticMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Routenplaner.ViewModels
{
    public class RoutenViewModel : BindableBase, IDisposable
    {
        #region private properties
        private IGeoCoding geoCoding;
        private IStaticMaps staticMaps;
        private IDirections directions;

        public EventOriginChanging OriginChanging;
        public EventDestinationChanging DestinationChanging;

        public bool DisplayMapCanExecute;
        public bool DisplayDirectionsCanExecute = true;

        public RoutenModel RoutenModel { get; set; }
        #endregion private properties

        #region Constructor
        /// <summary>
        /// Constructor of RoutenViewModel
        /// </summary>
        /// <param name="geoCoding"></param>
        /// <param name="staticMaps"></param>
        public RoutenViewModel(IGeoCoding geoCoding, IStaticMaps staticMaps, IDirections directions)
        {
            this.geoCoding = geoCoding;
            this.staticMaps = staticMaps;
            this.directions = directions;

            OriginChanging = new EventOriginChanging();
            OriginChanging.OriginChangingEvent += OnOriginChanging;

            DestinationChanging = new EventDestinationChanging();
            DestinationChanging.DestinationChangingEvent += OnDesinationChanging;

            DisplayMapCommand = new DelegateCommand(DisplayMap, CanDisplayMap);
            DisplayDirectionsCommand = new DelegateCommand(DisplayDirections, CanDisplayDirections);
            ClearFieldsCommand = new DelegateCommand(ClearFields, CanClearFields);

            RoutenModel = new RoutenModel(OriginChanging, DestinationChanging)
            {
                Origin = "Breiteweg 17, 30, 06 Bern",
                Waypoint = "Ziegelfeldstrasse, 4600 Olten",
                Destination = "Zürich Flughafen, 8302 Kloten"
            };
        }

        #endregion Constructor

        #region EventHandler
        private void OnOriginChanging(object sender, OriginChangingEventArgs args)
        {
            DisplayMapCanExecute = args.CanDisplay;
            DisplayMapCommand.RaiseCanExecuteChanged();
        }

        private void OnDesinationChanging(object sender, OriginChangingEventArgs args)
        {
            DisplayDirectionsCanExecute = args.CanDisplay;
            DisplayDirectionsCommand.RaiseCanExecuteChanged();
        }
        #endregion EventHandler

        #region DelegateCommands

        #region ClearFields
        public DelegateCommand ClearFieldsCommand { get; private set; }

        private bool CanClearFields()
        {
            return true;
        }

        private void ClearFields()
        {
            RoutenModel.Origin = String.Empty;
            RoutenModel.Waypoint = String.Empty;
            RoutenModel.Destination = String.Empty;
            RoutenModel.MapView = null;
        }
        #endregion ClearFields

        #region DisplayDirections
        public DelegateCommand DisplayDirectionsCommand { get; private set; }

        private bool CanDisplayDirections()
        {
            return DisplayDirectionsCanExecute;
        }

        private async void DisplayDirections()
        {
            RoutenModel.MapView = null;
            RoutenModel.Directions = string.Empty;
            RoutenModel.Error = string.Empty;

            if (!string.IsNullOrEmpty(RoutenModel.Origin) && !string.IsNullOrEmpty(RoutenModel.Destination))
            {
                var addresses = await GetAddresses();
                var dto = await directions.GetDirections(addresses);
                if (dto.Routes.status != "OK")
                {
                    RoutenModel.Error = "Fehler von Google: " + dto.Routes.status + " Bitte geben Sie das Feld Via genauer an!";
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append(dto.Routes.routes[0].legs[0].distance.text);
                    sb.Append("\t");
                    sb.Append(dto.Routes.routes[0].legs[0].duration.text);
                    sb.Append(Environment.NewLine);

                    foreach (var item in dto.Routes.routes[0].legs[0].steps)
                    {
                        sb.Append(item.distance.text);
                        sb.Append("\t");
                        sb.Append(item.duration.text);
                        if (item.duration.text.Contains("Minuten")) sb.Append("\t");
                        else sb.Append("\t\t");
                        sb.Append(StripHtml(item.html_instructions));
                        sb.Append(Environment.NewLine);
                    }
                    RoutenModel.Directions = sb.ToString();
                }
            }
            else
            {
                RoutenModel.Error = "Es müssen zumindest die Felder 'von' und 'nach' ausgefüllt sein.";
            }
        }

        #endregion DisplayDirections

        #region DisplayMap
        public DelegateCommand DisplayMapCommand { get; private set; }

        private bool CanDisplayMap()
        {
            return DisplayMapCanExecute;
        }

        private async void DisplayMap()
        {
            var addresses = await GetAddresses();
            if (addresses.Count() > 0)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                var dto = await staticMaps.GetMap(addresses);
                if (string.IsNullOrEmpty(dto.Error))
                {
                    bitmap.StreamSource = dto.Stream;
                    bitmap.EndInit();
                    RoutenModel.MapView = bitmap;
                    RoutenModel.Error = string.Empty;
                    RoutenModel.Directions = string.Empty;
                }
                else
                {
                    RoutenModel.Error = dto.Error;
                }
            }
        }
        #endregion DisplayMap

        #endregion DelegateCommands

        #region private functions
        private string StripHtml(string Txt)
        {
            return Regex.Replace(Txt, "<(.|\\n)*?>", string.Empty);
        }

        private async Task<List<LocationDto>> GetAddresses()
        {
            string origin = RoutenModel.Origin;
            string via = RoutenModel.Waypoint;
            string destination = RoutenModel.Destination;

            if (string.IsNullOrEmpty(origin) && string.IsNullOrEmpty(destination) && string.IsNullOrEmpty(via))
            {
                RoutenModel.Error = "Fehler: Es muss mindestens eines der Felder Von, Via oder Nach belegt sein!";
                return null;
            }

            List<LocationDto> addresses = new List<LocationDto>();
            IEnumerable<LocationDto> address = null;

            if (!string.IsNullOrEmpty(origin))
            {
                address = await geoCoding.GetAdresses(origin);
                var dto = address.First();
                if (string.IsNullOrEmpty(dto.Error))
                {
                    dto.LocationType = LocationType.Origin;
                    addresses.Add(dto);
                    RoutenModel.Origin = dto.FormatedAddress;
                }
                else
                {
                    RoutenModel.Error = dto.Error;
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(via))
            {
                address = await geoCoding.GetAdresses(via);
                var dto = address.First();
                if (string.IsNullOrEmpty(dto.Error))
                {
                    dto.LocationType = LocationType.WayPoint;
                    addresses.Add(dto);
                    RoutenModel.Waypoint = dto.FormatedAddress;
                }
                else
                {
                    RoutenModel.Error = dto.Error;
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(destination))
            {
                address = await geoCoding.GetAdresses(destination);
                var dto = address.First();
                if (string.IsNullOrEmpty(dto.Error))
                {
                    dto.LocationType = LocationType.Destination;
                    addresses.Add(dto);
                    RoutenModel.Destination = dto.FormatedAddress;
                }
                else
                {
                    RoutenModel.Error = dto.Error;
                    return null;
                }
            }
            return addresses;
        }
    
        #endregion private functions

        #region Dispose
        public void Dispose()
        {
            OriginChanging.OriginChangingEvent -= OnOriginChanging;
            DestinationChanging.DestinationChangingEvent -= OnDesinationChanging;
        }
        #endregion Dispose
    }
}
