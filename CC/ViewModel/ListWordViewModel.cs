using CC.Model;

namespace CC.ViewModel
{
    /// <summary>
    /// Модель представления слова списка терминов.
    /// </summary>
    public class ListWordViewModel : BaseViewModel
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ListWordViewModel"/>
        /// с заданным экземпляром <seealso cref="ListWordModel"/>.
        /// </summary>
        /// <param name="model">Модель слова списка терминов.</param>
        public ListWordViewModel(ListWordModel model)
        {
            Model = model;
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ListWordViewModel"/>.
        /// </summary>
        public ListWordViewModel()
        {
            Model = new ListWordModel();
        }

        /// <summary>
        /// Возвращает или задает модель данных <seealso cref="ListWordModel"/>.
        /// </summary>
        public ListWordModel Model { get; private set; }

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
        /// Возвращает значение длины <seealso cref="Answer"/>.
        /// </summary>
        public byte Length { get { return Model.Length; } }
    }
}