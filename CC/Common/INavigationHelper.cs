using Windows.UI.Xaml.Controls;

namespace CC.Common
{
    /// <summary>
    /// Интерфейс помощника навигации внутри приложения.
    /// </summary>
    public interface INavigationHelper
    {
        /// <summary>
        /// Текущий <see cref="Frame"/> приложения.
        /// </summary>
        Frame CurrentFrame { get; }
    }
}