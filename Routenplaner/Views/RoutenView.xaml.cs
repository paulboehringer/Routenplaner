using Routenplaner.ViewModels;
using Services.Directions;
using Services.GeoCoding;
using Services.StaticMaps;
using System.Windows;

namespace Routenplaner.Views
{
    /// <summary>
    /// Interaktionslogik für RoutenModel.xaml
    /// </summary>
    public partial class RoutenView : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="geoCoding"></param>
        /// <param name="staticMaps"></param>
        /// <param name="directions"></param>
        public RoutenView(IGeoCoding geoCoding, IStaticMaps staticMaps, IDirections directions)
        {
            InitializeComponent();
            DataContext = new RoutenViewModel(geoCoding, staticMaps, directions);
            DataGrid.DataContext = DataContext;
        }

    }
}
