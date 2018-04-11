using CC.ViewModel;

namespace CC.Model
{
    /// <summary>
    /// Модель ответа.
    /// </summary>
    public class FillingAnswerModel : BaseViewModel
    {
        private string _filled, _needed;

        /// <summary>
        /// Заполненный пользователем ответ.
        /// </summary>
        public string Filled
        {
            get { return _filled; }
            set { Set(ref _filled, value?.ToLower()); }
        }

        /// <summary>
        /// Требуемый ответ.
        /// </summary>
        public string Needed
        {
            get { return _needed; }
            set { Set(ref _needed, value); }
        }
    }
}
