using Services.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.GeoCoding
{
    /// <summary>
    /// Interface for GeoCoding service
    /// </summary>
    public interface IGeoCoding
    {
        Task<IEnumerable<LocationDto>> GetAdresses(string data);
    }
}
