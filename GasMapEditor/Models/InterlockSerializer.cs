using GasMapEditor.Services;
using System.Collections.Generic;
using System.Linq;

namespace GasMapEditor.Models;

internal class InterlockSerializer
{
    public InterlockSerializer() { }

    public InterlockSerializer(IInterlock interlock)
    {
        InterlockId = interlock.InterlockId;
        Interlocks = interlock.Interlocks.Select(i => i.InterlockId);
    }

    public int InterlockId { get; set; }
    public IEnumerable<int> Interlocks { get; set; } 
}

