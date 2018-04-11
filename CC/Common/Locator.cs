using CC.ViewModel;

namespace CC.Common
{
    /// <summary>
    /// Представляет локатор помощника навигации. 
    /// </summary>
    public static class Locator
    {
        public static AboutViewModel AboutVM { get; set; } = new AboutViewModel();
        /// <summary>
        /// Модель представления корневой страницы.
        /// </summary>
        public static ShellViewModel ShellVM { get; set; } = new ShellViewModel();
        /// <summary>
        /// Модель представления редактора списков.
        /// </summary>
        public static ListEditorViewModel ListEditorVM { get; set; } = new ListEditorViewModel();
        /// <summary>
        /// Модель представления редактора сетки.
        /// </summary>
        public static GridEditorViewModel GridEditorVM { get; set; } = new GridEditorViewModel();
        /// <summary>
        /// Модель представления страницы заполнения кроссворда.
        /// </summary>
        public static FillingViewModel FillingVM { get; set; } = new FillingViewModel();

        /// <summary>
        /// Помощник навигации.
        /// </summary>
        public static INavigationHelper NavigationHelper { get; private set; }

        /// <summary>
        /// Инициализирует помощник навигации.
        /// </summary>
        /// <param name="navigationHelper">Экземпляр помощника навигации.</param>
        public static void InitializeNavigationHelper(INavigationHelper navigationHelper)
        {
            NavigationHelper = navigationHelper;
        }
    }
}