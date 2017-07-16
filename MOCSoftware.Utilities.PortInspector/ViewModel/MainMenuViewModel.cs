using System.Collections.ObjectModel;
using System.Linq;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class MainMenuViewModel : ViewModelBase
    {
        public ObservableCollection<MenuItemViewModel> MainMenu { get; set; }

        public MainMenuViewModel()
        {
            MainMenu = new ObservableCollection<MenuItemViewModel>();
        }

        internal void MergeMenus(params MenuItemCollection[] menus)
        {
            menus.ToList().ForEach(MergeMenu);
        }

        private void MergeMenu(MenuItemCollection menu)
        {
            menu.ForEach(MergeItem);
        }

        private void MergeItem(MenuItemViewModel menuItem)
        {
            if (string.IsNullOrWhiteSpace(menuItem.Parent))
                AddRootMenuItem(menuItem);
            else
            {
                var parent = MainMenu.FirstOrDefault(e => e.Header.ToLower().Equals(menuItem.Parent.ToLower()));
                if (parent != null)
                    AddChildMenuItem(menuItem, parent);
            }
        }

        private void AddRootMenuItem(MenuItemViewModel menuItem)
        {
            if (!MainMenu.Contains(menuItem))
                InsertMenuItem(MainMenu, menuItem);
        }

        private void AddChildMenuItem(MenuItemViewModel child, MenuItemViewModel parent)
        {
            if (!parent.MenuItems.Contains(child))
                InsertMenuItem(parent.MenuItems, child);
        }

        private void InsertMenuItem(ObservableCollection<MenuItemViewModel> itemList, MenuItemViewModel menuItem)
        {
            var previousItem = itemList.LastOrDefault(e => e.Parent == menuItem.Parent && e.Priority <= menuItem.Priority);
            if (previousItem == null && itemList.Count == 0)
                itemList.Add(menuItem);
            else
            {
                var positionIndex = itemList.IndexOf(previousItem);
                if (positionIndex == itemList.Count - 1)
                    itemList.Add(menuItem);
                else
                {
                    itemList.Insert(positionIndex + 1, menuItem);
                }
            }
        }

        public MenuItemCollection GetContextMenu(string contextMenuName)
        {
            MenuItemCollection result = null;

            if (!string.IsNullOrWhiteSpace(contextMenuName))
                result =
                    MainMenu.SelectMany(
                        e => e.MenuItems.Where(s => s.ContextMenuName != null && s.ContextMenuName.ToUpper().Equals(contextMenuName.ToUpper()))).
                        ToMenuItemCollection();

            return result ?? (result = new MenuItemCollection());
        }

        public MenuItemCollection GetMenuItemsByTag(string tag)
        {
            var result = MainMenu.Where(e => e.Tag.ToUpperInvariant().Equals(tag.ToUpperInvariant())).ToMenuItemCollection();
                result.AddRange(MainMenu.SelectMany(e => e.MenuItems.Where(f => f.Tag.ToUpperInvariant().Equals(tag.ToUpperInvariant())))
                    .ToMenuItemCollection());
            return result;
        }
    }
}
