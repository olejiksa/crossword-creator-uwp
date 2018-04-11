using System;
using System.Diagnostics;

namespace CC.Common
{
    /// <summary>
    /// Представляет команду.
    /// </summary>
    public class DelegateCommand : IChangedCommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Обработчик события, определяющий, можно ли выполнить действие после изменения.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="execute">Действие для выполнения.</param>
        /// <param name="canExecute">Определяет, можно ли выполнить действие.</param>
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute ?? (() => true);
        }

        /// <summary>
        /// Определяет, можно ли выполнить действие.
        /// </summary>
        /// <param name="parameter">Пустой параметр.</param>
        /// <returns><see cref="bool"/> или <see cref="false"/>.</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter = null)
        {
            try { return _canExecute(); }
            catch { return false; }
        }

        /// <summary>
        /// Определяет выполнение.
        /// </summary>
        /// <param name="parameter">Пустой параметр.</param>
        public void Execute(object parameter = null)
        {
            if (!CanExecute(parameter))
                return;
            try { _execute(); }
            catch { Debugger.Break(); }
        }

        /// <summary>
        /// Вызывает событие выполнения действия после изменения.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Представляет команду с параметром.
    /// </summary>
    /// <typeparam name="T">Тип.</typeparam>
    public class DelegateCommand<T> : IChangedCommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Обработчик события, определяющий, можно ли выполнить действие после изменения.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="execute">Действие для выполнения.</param>
        /// <param name="canExecute">Определяет, можно ли выполнить действие.</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute ?? (e => true);
        }

        /// <summary>
        /// Определяет, можно ли выполнить действие.
        /// </summary>
        /// <param name="parameter">Пустой параметр.</param>
        /// <returns><see cref="bool"/> или <see cref="false"/>.</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter = null)
        {
            try { return _canExecute(ConvertParameterValue(parameter)); }
            catch { return false; }
        }

        /// <summary>
        /// Определяет выполнение.
        /// </summary>
        /// <param name="parameter">Пустой параметр.</param>
        public void Execute(object parameter = null)
        {
            if (!CanExecute(parameter))
                return;
            try { _execute(ConvertParameterValue(parameter)); }
            catch { Debugger.Break(); }
        }

        private static T ConvertParameterValue(object parameter)
        {
            parameter = parameter is T ? parameter : Convert.ChangeType(parameter, typeof(T));
            return (T)parameter;
        }

        /// <summary>
        /// Вызывает событие выполнения действия после изменения.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
