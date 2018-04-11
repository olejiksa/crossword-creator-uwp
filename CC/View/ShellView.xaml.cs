using CC.Common;
using CC.ViewModel;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CC.View
{
    /// <summary>
    /// Представляет корневую страницу.
    /// </summary>
    public sealed partial class ShellView : Page, IBindableView<ShellViewModel>, INavigationHelper
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public ShellView()
        {
            InitializeComponent();
            Locator.InitializeNavigationHelper(this);
        }

        /// <summary>
        /// Модель представления.
        /// </summary>
        public ShellViewModel ViewModel
        {
            get { return Locator.ShellVM; }
            set { Locator.ShellVM = value; }
        }

        /// <summary>
        /// Внутренний <see cref="Frame"/> приложения.
        /// </summary>
        public Frame CurrentFrame => frame;
    }
}