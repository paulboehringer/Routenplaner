using Services.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.StaticMaps
{
    /// <summary>
    /// Interface for static maps service
    /// </summary>
    public interface IStaticMaps
    {
        Task<StreamDto> GetMap(IEnumerable<LocationDto> locations);
    }
}
