using CC.Common;
using CC.Model;
using CC.ViewModel;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CC.View
{
    /// <summary>
    /// Представляет страницу со списком вопросов.
    /// </summary>
    public sealed partial class QuestionsView : Page, IBindableView<FillingViewModel>
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public QuestionsView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                titleBlock.Margin = new Thickness(12, 3, 0, 0);

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
        public FillingViewModel ViewModel
        {
            get { return Locator.FillingVM; }
            set { Locator.FillingVM = value; }
        }

        private void words_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListWordViewModel word = (ListWordViewModel)e.ClickedItem;

            for (ushort i = 0; i < ViewModel.Items.Count; i++)
                ViewModel.Items[i].IsSelected = false;

            ViewModel.Items[word.ID - 1].IsSelected = true;
            ViewModel.SelectedItem = ViewModel.Items[word.ID - 1];
        }

        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
                words.SelectedIndex = ViewModel.SelectedItem.ID - 1;
        }
    }
}
