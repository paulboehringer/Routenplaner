
namespace Services.Core
{
    public class DirectionDto
    {
        public string Directions { get; set; }
        public string Error { get; set; }
        public object Parsed { get; set; }
        public RootObject Routes { get; set; }
    }
}
