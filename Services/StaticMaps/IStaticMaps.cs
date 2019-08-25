using Services.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services.StaticMaps
{
    public interface IStaticMaps
    {
        Task<StreamDto> GetMap(IEnumerable<LocationDto> locations);
    }
}
