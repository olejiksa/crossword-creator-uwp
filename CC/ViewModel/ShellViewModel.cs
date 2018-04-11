using CC.Common;
using CC.Helpers;
using CC.Model;
using CC.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace CC.ViewModel
{
    /// <summary>
    /// Модель представления корневой страницы.
    /// </summary>
    public class ShellViewModel : BaseViewModel
    {
        private ObservableCollection<MenuItem> menuItems = new ObservableCollection<MenuItem>();
        private MenuItem selectedMenuItem, selectedBottomMenuItem;
        private string commonMenuItemTitle;
        private ObservableCollection<MenuItem> bottomMenuItems = new ObservableCollection<MenuItem>();
        private bool isSplitViewPaneOpen;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public ShellViewModel()
        {
            ToggleSplitViewPaneCommand = new DelegateCommand(() => IsSplitViewPaneOpen = !IsSplitViewPaneOpen);

            MenuItems.Add(new MenuItem { Icon = "\uE10F", Title = StringHelper.ToString("HomeSplitViewItem"), PageType = typeof(HomeView) });
            MenuItems.Add(new MenuItem { Icon = "\uE1EE", Title = StringHelper.ToString("ListEditorSplitViewItem"), PageType = typeof(ListEditorView) });
            MenuItems.Add(new MenuItem { Icon = "\uE80A", Title = StringHelper.ToString("GridEditorSplitViewItem"), PageType = typeof(GridEditorView) });
            MenuItems.Add(new MenuItem { Icon = "\uE104", Title = StringHelper.ToString("FillingSplitViewItem"), PageType = typeof(FillingView) });

            BottomMenuItems.Add(new MenuItem { Icon = "\uE946", Title = StringHelper.ToString("AboutSplitViewItem"), PageType = typeof(AboutView) });

            SelectedMenuItem = MenuItems.First();
        }

        /// <summary>
        /// Команда открытия гамбургер-меню.
        /// </summary>
        public ICommand ToggleSplitViewPaneCommand { get; private set; }

        public event Action<bool> SplitViewPaneOpenChanged;

        /// <summary>
        /// Свойство открытости/закрытости гамбургер-меню.
        /// </summary>
        public bool IsSplitViewPaneOpen
        {
            get { return isSplitViewPaneOpen; }
            set
            {
                Set(ref isSplitViewPaneOpen, value);
                SplitViewPaneOpenChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Текущий выбранный элемент в гамбургер-меню.
        /// </summary>
        public MenuItem SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set
            {
                if (Set(ref selectedMenuItem, value) && value != null)
                {
                    CommonMenuItemTitle = value.Title;
                    OnSelectedMenuItemChanged(true);
                }
            }
        }

        /// <summary>
        /// Текущий выбранный элемент в нижней части гамбургер-меню.
        /// </summary>
        public MenuItem SelectedBottomMenuItem
        {
            get { return selectedBottomMenuItem; }
            set
            {
                if (Set(ref selectedBottomMenuItem, value) && value != null)
                {
                    CommonMenuItemTitle = value.Title;
                    OnSelectedMenuItemChanged(false);
                }
            }
        }

        public string CommonMenuItemTitle
        {
            get { return commonMenuItemTitle; }
            set
            {
                Set(ref commonMenuItemTitle, value);
            }
        }

        /// <summary>
        /// Выбранная страница гамбургер-меню.
        /// </summary>
        public Type SelectedPageType
        {
            get
            {
                return (selectedMenuItem ?? selectedBottomMenuItem)?.PageType;
            }
            set
            {
                SelectedMenuItem = menuItems.FirstOrDefault(m => m.PageType == value);
                SelectedBottomMenuItem = bottomMenuItems.FirstOrDefault(m => m.PageType == value);
            }
        }

        /// <summary>
        /// Коллекция навигационных элементов.
        /// </summary>
        public ObservableCollection<MenuItem> MenuItems
        {
            get { return menuItems; }
        }

        /// <summary>
        /// Коллекция нижних навигационных элементов.
        /// </summary>
        public ObservableCollection<MenuItem> BottomMenuItems
        {
            get { return bottomMenuItems; }
        }

        /// <summary>
        /// Срабатывает при изменении выделенного элемента.
        /// </summary>
        /// <param name="top">Основной или нижний список.</param>
        private void OnSelectedMenuItemChanged(bool top)
        {
            if (top)
                SelectedBottomMenuItem = null;
            else
                SelectedMenuItem = null;
            OnPropertyChanged(nameof(SelectedPageType));

            if (!IsWideState())
                IsSplitViewPaneOpen = false;
        }

        /// <summary>
        /// Растянутое ли окно или нет.
        /// </summary>
        /// <returns>true или false.</returns>
        private bool IsWideState()
        {
            return Window.Current.Bounds.Width >= 1024;
        }
    }
}