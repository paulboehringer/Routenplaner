using Services.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.GeoCoding
{
    public interface IGeoCoding
    {
        Task<IEnumerable<LocationDto>> GetAdresses(string data);
    }
}
