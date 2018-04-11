using CC.Common;
using CC.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

namespace CC.View
{
    /// <summary>
    /// Представляет страницу редактора сетки.
    /// </summary>
    public sealed partial class GridEditorView : Page, IBindableView<GridEditorViewModel>
    {
        private uint zIndex = ushort.MaxValue;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GridEditorView()
        {
            InitializeComponent();

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                titleBlock.Margin = new Thickness(0, 3, 0, 0);
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
        public GridEditorViewModel ViewModel
        {
            get { return Locator.GridEditorVM; }
            set { Locator.GridEditorVM = value; }
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

            if (e.NavigationMode != NavigationMode.Back && e.Parameter is IStorageFile)
                ViewModel.OpenGrid((StorageFile)e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ApplicationView.GetForCurrentView().Title = string.Empty;
        }

        #region Public methods
        /// <summary>
        /// Выделяет элемент, делая его активным.
        /// </summary>
        /// <param name="sender">Выделенный элемент управления.</param>
        public ushort Select(object sender)
        {
            foreach (object item in itemsControl.Items)
            {
                DependencyObject a = itemsControl.ContainerFromItem(item);
                List<Control> b = AllChildren(a);
                (b[0] as ItemsControl).ItemsPanelRoot.Background = (SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            }

            DependencyObject _Container = sender as DependencyObject;
            List<Control> _Children = AllChildren(_Container);

            ItemsControl subItemsControl = _Children.OfType<ItemsControl>().ElementAt(0);
            TextBlock text = ((sender as StackPanel).Children[0] as Grid).Children[0] as TextBlock;
            ((StackPanel)subItemsControl.ItemsPanelRoot).Background = (SolidColorBrush)Application.Current.Resources["SystemControlBackgroundAccentBrush"];
            ((StackPanel)subItemsControl.ItemsPanelRoot).Background.Opacity = 0.5;

            itemsControl.ContainerFromIndex(int.Parse(text.Text)).SetValue(Canvas.ZIndexProperty, zIndex++);

            ushort selectedIndex = ushort.Parse(text.Text);
            ViewModel.SelectedItem = ViewModel.Items[selectedIndex];
            return selectedIndex;
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

        #region Private methods
        /// <summary>
        /// Срабатывает при нажатии на элемент.
        /// </summary>
        private void PanelTapped(object sender, TappedRoutedEventArgs e)
        {
            Select(sender);
        }

        /// <summary>
        /// Срабатывает при перемещении элемента.
        /// </summary>
        private void PanelManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!e.IsInertial)
            {
                ushort id = Select(sender);
                
                DependencyObject myEllipse = VisualTreeHelper.GetParent(sender as DependencyObject);
                ContentPresenter panel = (ContentPresenter)myEllipse;

                const double digit = 12.5;

                if (e.Position.X >= digit)
                    Canvas.SetLeft(panel, Canvas.GetLeft(panel) + 25);
                else if (e.Position.X < -digit)
                    Canvas.SetLeft(panel, Canvas.GetLeft(panel) - 25);

                if (e.Position.Y >= digit)
                    Canvas.SetTop(panel, Canvas.GetTop(panel) + 25);
                else if (e.Position.Y < -digit)
                    Canvas.SetTop(panel, Canvas.GetTop(panel) - 25);

                ViewModel.Items[id].X = Canvas.GetLeft(panel);
                ViewModel.Items[id].Y = Canvas.GetTop(panel);
                ViewModel.IsDirty = true;
            }
        }

        /// <summary>
        /// Срабатывает при двойном нажатии левой кнопкой мыши.
        /// </summary>
        private void PanelDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Orientation orientation = ViewModel.Items[Select(sender)].Orientation;
            ViewModel.Items[Select(sender)].Orientation = orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;

            ViewModel.IsDirty = true;
        }

        /// <summary>
        /// Срабатывает при нажатии правой кнопкой мыши.
        /// </summary>
        private void PanelRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowFlyout((FrameworkElement)sender);
        }

        /// <summary>
        /// Срабатывает при удерживании пальца.
        /// </summary>
        private void PanelHolding(object sender, HoldingRoutedEventArgs e)
        {
            ShowFlyout((FrameworkElement)sender);
        }

        /// <summary>
        /// Вызывает отображение контекстного меню.
        /// </summary>
        /// <param name="sender">Объект, с которым связано меню.</param>
        private void ShowFlyout(FrameworkElement sender)
        {
            Select(sender);
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(sender);
            flyoutBase.ShowAt(sender);
        }
        #endregion
    }
}