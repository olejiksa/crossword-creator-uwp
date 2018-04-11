using CC.Common;
using CC.View;
using System.Windows.Input;

namespace CC.ViewModel
{
    /// <summary>
    /// Представляет модель представления домашней страницы.
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        #region Constructor
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public HomeViewModel() { }
        #endregion

        #region Commands
        /// <summary>
        /// Команда навигации на страницу редактора списков.
        /// </summary>
        public ICommand GoToListEditorView { get; private set; } = new DelegateCommand(() =>
        {
            Locator.NavigationHelper.CurrentFrame.Navigate(typeof(ListEditorView));
        });

        /// <summary>
        /// Команда навигации на страницу редактора сетки.
        /// </summary>
        public ICommand GoToGridEditorView { get; private set; } = new DelegateCommand(() =>
        {
            Locator.NavigationHelper.CurrentFrame.Navigate(typeof(GridEditorView));
        });

        /// <summary>
        /// Команда навигации на страницу заполнения кроссворда.
        /// </summary>
        public ICommand GoToFillingView { get; private set; } = new DelegateCommand(() =>
        {
            Locator.NavigationHelper.CurrentFrame.Navigate(typeof(FillingView));
        });
        #endregion
    }
}