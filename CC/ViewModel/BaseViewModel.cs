using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CC.ViewModel
{
    /// <summary>
    /// Абстрактный класс, реализующий интерфейс <see cref="INotifyPropertyChanged"/>.
    /// Используется для реализации события обновления свойств.
    /// Также применяется для установки значения свойству с более упрощенным синтаксисом.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Возникает в случае изменения значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Уведомляет клиентов об изменении свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Устанавливает значение свойству.
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="storage">Хранилище.</param>
        /// <param name="value">Значение.</param>
        /// <param name="propertyName">Имя свойства (может не использоваться).</param>
        /// <returns>Возвращает <see cref="true"/> в случае успешной передачи значения.</returns>
        public bool Set<T>(ref T storage, T value, [CallerMemberName()]string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}