using CC.ViewModel;

namespace CC.Model
{
    /// <summary>
    /// Представляет символ.
    /// </summary>
    public class SymbolItem : BaseViewModel
    {
        private char _symbol;
        private bool _isVisible;

        /// <summary>
        /// Символ.
        /// </summary>
        public char Symbol
        {
            get { return _symbol; }
            set
            {
                Set(ref _symbol, value);
                IsVisible = value != ' ';
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(ref _isVisible, value); }
        }
    }
}