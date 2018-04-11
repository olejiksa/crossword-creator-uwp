using CC.Common;
using CC.ViewModel;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CC.View
{
    /// <summary>
    /// Представляет страницу добавления нового элемента редактора списка.
    /// </summary>
    public sealed partial class AddListWordView : Page, IBindableView<ListEditorViewModel>
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public AddListWordView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                titleBlock.Margin = titleBlock2.Margin = new Thickness(12, 3, 0, 0);

            page.SizeChanged += Page_SizeChanged;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (page.ActualWidth >= 976)
                Locator.NavigationHelper.CurrentFrame.GoBack();
        }

        /// <summary>
        /// Модель представления.
        /// </summary>
        public ListEditorViewModel ViewModel
        {
            get { return Locator.ListEditorVM; }
            set { Locator.ListEditorVM = value; }
        }
    }
}