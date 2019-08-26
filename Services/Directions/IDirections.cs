using Services.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Directions
{
    /// <summary>
    /// Interface for Directions service
    /// </summary>
    public interface IDirections
    {
        Task<DirectionDto> GetDirections(IEnumerable<LocationDto> locations);
    }
}
