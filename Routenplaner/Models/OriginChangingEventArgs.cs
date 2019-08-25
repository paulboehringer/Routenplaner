using System;

namespace Routenplaner.Models
{
    public class OriginChangingEventArgs: EventArgs
    {
        public bool CanDisplay { get; set; }
    }
}
