
namespace Services.Core
{
    /// <summary>
    /// Data transfer object for directions
    /// </summary>
    public class DirectionDto
    {
        public string Directions { get; set; }
        public object Parsed { get; set; }
        public RootObject Routes { get; set; }
        public string Error { get; set; }
    }
}
