using CC.Common;
using CC.ViewModel;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CC.View
{
    /// <summary>
    /// Представляет домашнюю страницу.
    /// </summary>
    public sealed partial class HomeView : Page, IBindableView<HomeViewModel>
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public HomeView()
        {
            InitializeComponent();
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                titleBlock.Margin = new Thickness(0, 3, 0, 0);

            Locator.ShellVM.SplitViewPaneOpenChanged += ShellVM_SplitViewPaneOpenChanged;
        }

        private void ShellVM_SplitViewPaneOpenChanged(bool obj)
        {
            if (Window.Current.Bounds.Width >= 1024)
                titleGrid.Margin = obj ? new Thickness(0) : new Thickness(272, 0, 0, 0);
            else
                titleGrid.Margin = new Thickness(272, 0, 0, 0);

            inner.Margin = new Thickness(272, 0, 0, 0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Window.Current.Bounds.Width >= 1024)
                titleGrid.Margin = Locator.ShellVM.IsSplitViewPaneOpen ? new Thickness(0) : new Thickness(272, 0, 0, 0);
            else
                titleGrid.Margin = new Thickness(272, 0, 0, 0);

            inner.Margin = new Thickness(272, 0, 0, 0);
        }

        /// <summary>
        /// Модель представления домашней страницы.
        /// </summary>
        public HomeViewModel ViewModel { get; set; } = new HomeViewModel();
    }
}