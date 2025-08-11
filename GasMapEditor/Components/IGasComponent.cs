using GasMapEditor.Models;
using System.Collections.Generic;

namespace GasMapEditor.Components;

/// <summary>
/// 实现此接口，Toolbox才会自动添加该组件
/// </summary>
internal interface IGasMapElement { }

internal interface IGasMapComponent : IGasMapElement
{
    HashSet<Gas> CurrentGases { get; set; }

    /// <summary>
    /// 更新气路状态
    /// </summary>
    void Update();

    /// <summary>
    /// 抽真空
    /// </summary>
    void Vacuumize();
}
