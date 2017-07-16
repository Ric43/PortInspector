using System.Collections.Generic;
using MOCSoftware.Utilities.PortInspector.ViewModel;

namespace MOCSoftware.Utilities.PortInspector.Utility
{
    internal static class Extensions
    {
        internal static MenuItemCollection ToMenuItemCollection(this IEnumerable<MenuItemViewModel> collection)
        {
            var result = new MenuItemCollection();

            result.AddRange(collection);

            return result;
        }
    }
}
