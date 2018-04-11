using CC.Model;
using Windows.UI.Xaml.Controls;

namespace CC.ViewModel
{
    /// <summary>
    /// Модель представления слова на сетке.
    /// </summary>
    public class GridWordViewModel : BaseViewModel
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GridWordViewModel"/>
        /// с заданным экземпляром <seealso cref="GridWordModel"/>.
        /// </summary>
        /// <param name="model">Модель слова сетки.</param>
        public GridWordViewModel(GridWordModel model)
        {
            Model = model;
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GridWordViewModel"/>.
        /// </summary>
        public GridWordViewModel()
        {
            Model = new GridWordModel();
        }

        /// <summary>
        /// Возвращает или задает текущую модель данных <seealso cref="GridWordModel"/>.
        /// </summary>
        public GridWordModel Model { get; set; }

        /// <summary>
        /// Возвращает или задает значение ответа (в нижнем регистре).
        /// </summary>
        public string Answer
        {
            get { return Model.Answer; }
            set
            {
                string _item = value.ToLower();
                if (_item == Model.Answer)
                    return;
                Model.Answer = _item;
                OnPropertyChanged(nameof(Answer));
            }
        }

        /// <summary>
        /// Возвращает или задает значение вопроса.
        /// </summary>
        public string Question
        {
            get { return Model.Question; }
            set
            {
                if (value == Model.Question)
                    return;
                Model.Question = value;
                OnPropertyChanged(nameof(Question));
            }
        }

        /// <summary>
        /// Возвращает или задает значение идентификатора слова.
        /// </summary>
        public ushort ID
        {
            get { return Model.ID; }
            set
            {
                if (value == Model.ID)
                    return;
                Model.ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        /// <summary>
        /// Возвращает или задает позицию слова по оси X относительно верхнего левого угла.
        /// </summary>
        public double X
        {
            get { return Model.X; }
            set
            {
                if (value == Model.X)
                    return;
                Model.X = value;
                OnPropertyChanged(nameof(X));
            }
        }

        /// <summary>
        /// Возвращает или задает позицию слова по оси Y относительно верхнего левого угла.
        /// </summary>
        public double Y
        {
            get { return Model.Y; }
            set
            {
                if (value == Model.Y)
                    return;
                Model.Y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        /// <summary>
        /// Возвращает или задает ориентацию слова.
        /// </summary>
        public Orientation Orientation
        {
            get { return Model.Orientation; }
            set
            {
                if (value == Model.Orientation)
                    return;
                Model.Orientation = value;
                OnPropertyChanged(nameof(Orientation));
            }
        }
    }
}