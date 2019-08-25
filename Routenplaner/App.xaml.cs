using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Routenplaner.Views;
using Services.Directions;
using Services.GeoCoding;
using Services.StaticMaps;
using System.Windows;

namespace Routenplaner
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<RoutenView>();
        }

        /// <summary>
        /// Resolve the needed interfaces
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IGeoCoding, GeoCoding>();
            containerRegistry.RegisterSingleton<IStaticMaps, StaticMaps>();
            containerRegistry.RegisterSingleton<IDirections, Directions>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
        }
    }
}
