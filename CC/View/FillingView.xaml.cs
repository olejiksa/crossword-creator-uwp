using CC.Common;
using CC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

namespace CC.View
{
    /// <summary>
    /// Представляет страницу заполнения кроссворда.
    /// </summary>
    public sealed partial class FillingView : Page, IBindableView<FillingViewModel>
    {
        private uint zIndex = ushort.MaxValue;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public FillingView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                titleBlock.Margin = new Thickness(12, 3, 0, 0);
                fileName.Margin = new Thickness(24, -1, 0, 0);
            }
            Locator.ShellVM.SplitViewPaneOpenChanged += ShellVM_SplitViewPaneOpenChanged;
        }

        private void ShellVM_SplitViewPaneOpenChanged(bool obj)
        {
            if (Window.Current.Bounds.Width >= 1024)
                titleGrid.Margin = obj ? new Thickness(0) : new Thickness(272, 0, 0, 0);
            else
                titleGrid.Margin = new Thickness(272, 0, 0, 0);

            crosswordGrid.Margin = emptyPanel.Margin = new Thickness(272, 0, 0, 0);
        }

        /// <summary>
        /// Модель представления.
        /// </summary>
        public FillingViewModel ViewModel
        {
            get { return Locator.FillingVM; }
            set { Locator.FillingVM = value; }
        }

        /// <summary>
        /// Срабатывает при навигации на страницу.
        /// </summary>
        /// <param name="e">Передаваемые данные навигации.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Window.Current.Bounds.Width >= 1024)
                titleGrid.Margin = Locator.ShellVM.IsSplitViewPaneOpen ? new Thickness(0) : new Thickness(272, 0, 0, 0);
            else
                titleGrid.Margin = new Thickness(272, 0, 0, 0);
            crosswordGrid.Margin = emptyPanel.Margin = new Thickness(272, 0, 0, 0);

            ApplicationView.GetForCurrentView().Title = ViewModel.GridFileName;

            ViewModel.OnNavigatedTo(this);
            if (ViewModel.SelectedItem != null)
                listView.SelectedIndex = ViewModel.SelectedItem.ID - 1;
            if (e.NavigationMode != NavigationMode.Back && e.Parameter is IStorageFile)
                ViewModel.OpenGrid((StorageFile)e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ApplicationView.GetForCurrentView().Title = string.Empty;
            ViewModel.OnNavigatedFrom();
        }

        #region Public methods
        /// <summary>
        /// Выделяет элемент, делая его активным.
        /// </summary>
        /// <param name="sender">Выделенный элемент управления.</param>
        public ushort Select(object sender)
        {
            DependencyObject _Container = sender as DependencyObject;
            List<Control> _Children = AllChildren(_Container);
            TextBlock text = ((sender as StackPanel).Children[0] as Grid).Children[0] as TextBlock;
            return ushort.Parse((int.Parse(text.Text) - 1).ToString());
        }

        /// <summary>
        /// Находит все дочерние элементы объекта-родителя зависимостей.
        /// </summary>
        /// <param name="parent">Объект.</param>
        /// <returns>Список элементов.</returns>
        public List<Control> AllChildren(DependencyObject parent)
        {
            List<Control> list = new List<Control>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Control)
                    list.Add(child as Control);
                list.AddRange(AllChildren(child));
            }

            return list;
        }
        #endregion

        /// <summary>
        /// Срабатывает при нажатии на элемент.
        /// </summary>
        private void PanelTapped(object sender, TappedRoutedEventArgs e)
        {
            listView.SelectedIndex = Select(sender);

            for (ushort i = 0; i < itemsControl.Items.Count; i++)
                ViewModel.Items[i].IsSelected = false;

            ViewModel.Items[listView.SelectedIndex].IsSelected = true;
            ViewModel.SelectedItem = ViewModel.Items[listView.SelectedIndex];
            itemsControl.ContainerFromIndex(listView.SelectedIndex).SetValue(Canvas.ZIndexProperty, zIndex++);

            answerBox.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Срабатывает при выборе элемента списка.
        /// </summary>
        private async void ListItemClick(object sender, ItemClickEventArgs e)
        {
            ListWordViewModel word = (ListWordViewModel)e.ClickedItem;

            for (ushort i = 0; i < itemsControl.Items.Count; i++)
                ViewModel.Items[i].IsSelected = false;

            ViewModel.Items[word.ID - 1].IsSelected = true;
            ViewModel.SelectedItem = ViewModel.Items[word.ID - 1];
            itemsControl.ContainerFromIndex(word.ID - 1).SetValue(Canvas.ZIndexProperty, zIndex++);

            await Task.Delay(1);
            answerBox.Focus(FocusState.Programmatic);
        }
    }
}