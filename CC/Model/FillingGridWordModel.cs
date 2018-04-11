using CC.ViewModel;
using Windows.UI.Xaml.Controls;

namespace CC.Model
{
    /// <summary>
    /// Модель заполняемого слова.
    /// </summary>
    public class FillingGridWordModel : BaseViewModel
    {
        private FillingAnswerModel _answer;
        private string _question;
        private ushort _id;
        private double _x, _y;
        private Orientation _orientation;
        private bool _isSelected;

        /// <summary>
        /// Возвращает или задает значение ответа (в нижнем регистре).
        /// </summary>
        public FillingAnswerModel Answer
        {
            get { return _answer; }
            set { Set(ref _answer, value); }
        }

        /// <summary>
        /// Возвращает или задает значение вопроса.
        /// </summary>
        public string Question
        {
            get { return _question; }
            set { Set(ref _question, value); }
        }

        /// <summary>
        /// Возвращает или задает значение идентификатора слова.
        /// </summary>
        public ushort ID
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        /// <summary>
        /// Возвращает или задает позицию слова по оси X относительно верхнего левого угла.
        /// </summary>
        public double X
        {
            get { return _x; }
            set { Set(ref _x, value); }
        }

        /// <summary>
        /// Возвращает или задает позицию слова по оси Y относительно верхнего левого угла.
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { Set(ref _y, value); }
        }

        /// <summary>
        /// Возвращает или задает ориентацию слова.
        /// </summary>
        public Orientation Orientation
        {
            get { return _orientation; }
            set { Set(ref _orientation, value); }
        }

        /// <summary>
        /// Возвращает или задает значение выделенности слова.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }
    }
}
