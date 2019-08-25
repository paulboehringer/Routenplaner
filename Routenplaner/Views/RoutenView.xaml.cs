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

        public RoutenView(IGeoCoding geoCoding, IStaticMaps staticMaps, IDirections directions)
        {
            InitializeComponent();
            DataContext = new RoutenViewModel(geoCoding, staticMaps, directions);
            DataGrid.DataContext = DataContext;
        }

    }
}
