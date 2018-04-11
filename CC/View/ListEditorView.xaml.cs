using CC.Common;
using CC.Enums;
using CC.Helpers;
using CC.ViewModel;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CC.View
{
    /// <summary>
    /// Представляет страницу редактора списков.
    /// </summary>
    public sealed partial class ListEditorView : Page, IBindableView<ListEditorViewModel>
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public ListEditorView()
        {
            InitializeComponent();

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                titleBlock.Margin = new Thickness(0, 3, 0, 0);
                fileName.Margin = new Thickness(24, -1.5, 0, 0);
            }

            page.Loaded += Page_Loaded;
            Locator.ShellVM.SplitViewPaneOpenChanged += ShellVM_SplitViewPaneOpenChanged;
        }

        private void ShellVM_SplitViewPaneOpenChanged(bool obj)
        {
            if (Window.Current.Bounds.Width >= 1024)
                titleGrid.Margin = obj ? new Thickness(0) : new Thickness(272, 0, 0, 0);
            else
                titleGrid.Margin = new Thickness(272, 0, 0, 0);

            words.Margin = emptyPanel.Margin = new Thickness(272, 0, 0, 0);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateAddingState();
        }

        /// <summary>
        /// Модель представления.
        /// </summary>
        public ListEditorViewModel ViewModel
        {
            get { return Locator.ListEditorVM; }
            set { Locator.ListEditorVM = value; }
        }

        /// <summary>
        /// Срабатывает при навигации на страницу.
        /// </summary>
        /// <param name="e">Передаваемые данные навигации.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Window.Current.Bounds.Width >= 1024)
                titleGrid.Margin = Locator.ShellVM.IsSplitViewPaneOpen ? new Thickness(0) : new Thickness(272, 0, 0, 0);
            else
                titleGrid.Margin = new Thickness(272, 0, 0, 0);
            words.Margin = emptyPanel.Margin = new Thickness(272, 0, 0, 0);

            ApplicationView.GetForCurrentView().Title = ViewModel.FileName;

            if (e.NavigationMode != NavigationMode.Back
                && e.Parameter is StorageFile && (ViewModel.IsDirty && ViewModel.FileName != StringHelper.ToString("UntitledList")
                || ViewModel.FileName == StringHelper.ToString("UntitledList") && ViewModel.Items.Count > 0))
            {
                MessageDialogResult result = await Messages.SaveTheChangesAsync(ViewModel.FileName);

                switch (result)
                {
                    case MessageDialogResult.Yes: await ViewModel.SaveButtonClick(); break;
                    case MessageDialogResult.No: break;
                    case MessageDialogResult.Cancel: return;
                }

                ViewModel.OpenListFile((StorageFile)e.Parameter);
            }
            else if (e.NavigationMode != NavigationMode.Back
                && e.Parameter is StorageFile)
                ViewModel.OpenListFile((StorageFile)e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ApplicationView.GetForCurrentView().Title = string.Empty;
        }
    }
}