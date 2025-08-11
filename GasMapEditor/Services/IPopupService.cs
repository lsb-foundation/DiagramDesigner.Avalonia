using GasMapEditor.Models;
using System.Threading.Tasks;

namespace GasMapEditor.Services;

internal interface IPopupService
{
    Task<bool> ShowDialog(PopupDataContainer popupDataContainer);
}
