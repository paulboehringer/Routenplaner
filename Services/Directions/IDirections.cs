using Services.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Directions
{
    public interface IDirections
    {
        Task<DirectionDto> GetDirections(IEnumerable<LocationDto> locations);
    }
}
