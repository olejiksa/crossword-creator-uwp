using CC.ViewModel;

namespace CC.View
{
    /// <summary>
    /// Представляет интерфейс для создания связи между представлением и моделью представления.
    /// </summary>
    /// <typeparam name="T">Модель представления.</typeparam>
    public interface IBindableView<T> where T : BaseViewModel
    {
        /// <summary>
        /// Экземпляр модели представления.
        /// </summary>
        T ViewModel { get; set; }
    }
}