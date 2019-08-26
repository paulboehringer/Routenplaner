using System;

namespace Routenplaner.Models
{
    /// <summary>
    /// EventArgs for field changing
    /// </summary>
    public class FieldChangingEventArgs: EventArgs
    {
        public bool CanDisplay { get; set; }
    }
}
