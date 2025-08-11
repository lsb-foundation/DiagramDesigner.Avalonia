using System.Collections.Generic;
using System.Linq;

namespace GasMapEditor.Services;

public interface IInterlock
{
    int InterlockId { get; set; }
    bool IsLocked { get; set; }
    Interlocks Interlocks { get; }
}

public class Interlocks : List<IInterlock>
{
    private static int _id = 0;

    public static int NextId => ++_id;

    public static void SetMaxId(int max)
    {
        _id = max;
    }

    public void AddInterlock(IInterlock interlock)
    {
        if (!Contains(interlock))
        {
            if (interlock.InterlockId == 0)
            {
                interlock.InterlockId = NextId;
            }
            Add(interlock);
        }
    }

    public void AddInterlocks(IEnumerable<IInterlock> interlocks)
    {
        foreach (var interlock in interlocks)
        {
            AddInterlock(interlock);
        }
    }

    public bool IsLocked => Count > 0 && this.Any(i => i.IsLocked);
}
